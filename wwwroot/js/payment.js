(function (app) {
    // Set up Stripe.js and Elements to use in checkout form
    var style = {
        base: {
            color: "#32325d",
            fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
            fontSmoothing: "antialiased",
            fontSize: "16px",
            "::placeholder": {
                color: "#aab7c4"
            }
        },
        invalid: {
            color: "#fa755a",
            iconColor: "#fa755a"
        },
    };

    var $btn, $alertTemplate, $alertPlaceHolder, $transactionFee, $totalAmount, $cardCountryText;

    function initCheckout(stripePublishableKey) {
        var stripe = Stripe(stripePublishableKey);
        var elements = stripe.elements();

        var form = document.getElementById('payment-form');
        $btn = $(form).find('button');
        $btn.prop('disabled', true);
        $alertPlaceHolder = $('#alert-placeholder');
        $alertTemplate = $alertPlaceHolder.children('div');
        $alertTemplate.removeClass('alert-success').addClass('show').remove();
        $transactionFee = $('#transaction-fee-text');
        $totalAmount = $('#total-amount-text');
        $cardCountryText = $('#card-country-text');

        var cardElement = elements.create('card', { style: style });
        cardElement.mount('#card-element');
        cardElement.on('change', cardElementChangeHandler);

        form.addEventListener('submit', function (event) {
            // We don't want to let default form submission happen here,
            // which would refresh the page.
            event.preventDefault();
            $btn.prop('disabled', true);

            stripe.createPaymentMethod({
                type: 'card',
                card: cardElement,
                billing_details: {
                    // Include any additional collected billing details.
                    name: 'Jhon Doe'
                },
            }).then(stripePaymentMethodHandler);
        });
    }

    function cardElementChangeHandler(event) {
        if (event.complete) {
            $btn.prop('disabled', false);
        } else {
            $btn.prop('disabled', true);
            $btn.text('Submit Payment');
        }
    }

    function stripePaymentMethodHandler(paymentMethodCreateResult) {
        if (paymentMethodCreateResult.error) {
            showAlert('Payment failed! ' + paymentMethodCreateResult.error.message, 'alert-danger');
        } else {
            fetch('/home/recalculateInvoice', {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    payment_method_id: paymentMethodCreateResult.paymentMethod.id,
                })
            }).then(function (result) {
                if (!result.ok) {
                    result.json().then(function (json) {
                        console.warn('Request failed: ' + JSON.stringify(json));
                    });
                    showAlert('Request failed with status ' + result.status, 'alert-danger');
                } else {
                    // Handle server response (see Step 4)
                    result.json().then(function (json) {
                        handleRecalculateInvoiceJson(json, paymentMethodCreateResult.paymentMethod.id);
                    });
                }

                $btn.prop('disabled', false);
            });
        }
    }

    function handleRecalculateInvoiceJson(json, paymentMethodId) {
        if (json.totalAmountFormatted !== getTotalAmount()) {
            showAlert('Invoice amount changed! According to the state of your courntry the total amount is ' + json.totalAmountFormatted + '. You was not charged yet.', 'alert-warning');

            setInnerTextNodeHtml($transactionFee, json.transactionFeeFormatted);
            setInnerTextNodeHtml($totalAmount, json.totalAmountFormatted);
            setInnerTextNodeHtml($cardCountryText, json.cardCountry);
            $btn.text('Pay ' + json.totalAmountFormatted + ' ' + json.currency);
        } else {
            fetch('/home/pay', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    payment_method_id: paymentMethodId,
                })
            }).then(function (result) {
                handlePayResponse(result);
            });
        }
    }

    function getTotalAmount() {
        return getInnerTextNodes($totalAmount).text().trim();
    }

    function setInnerTextNodeHtml($element, text) {
        var textNode = document.createTextNode(text);
        getInnerTextNodes($element).replaceWith($(textNode));
    }

    function getInnerTextNodes($element) {
        return $element.contents().filter(function () {
            return this.nodeType === 3; //Node.TEXT_NODE
        });
    }

    function handlePayResponse(fetchResult) {
        if(!fetchResult.ok) {
            showAlert('Paymet request failed with status ' + fetchResult.status, 'alert-danger');
            return;
        }

        showAlert('Payment succeeded', 'alert-success');

        Swal.fire(
            'Transaction succeeded',
            'Your payment has been competed successfully',
            'success')
            .then(function (result) {
                window.location.reload();
            });
    }

    function showAlert(text, className) {
        var $alert = $alertTemplate.clone();
        $alert.addClass(className);
        $alert.appendTo($alertPlaceHolder);
        $alert.prepend(text);
    }

    app.initCheckout = initCheckout;
})(app);
