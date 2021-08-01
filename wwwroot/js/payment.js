(function(app){
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

    var $btn, $alertTemplate, $alertPlaceHolder;
    var totalAmount;

    function initCheckout(stripePublishableKey, totalAmountToPay) {
        totalAmount = totalAmountToPay;
        
        var stripe = Stripe(stripePublishableKey);
        var elements = stripe.elements();
        
        var form = document.getElementById('payment-form');
        $btn = $(form).find('button');
        $btn.prop('disabled', true);
        $alertPlaceHolder = $('#alert-placeholder');
        $alertTemplate = $alertPlaceHolder.children('div');
        $alertTemplate.removeClass('alert-success').addClass('show').remove();
        
        var cardElement = elements.create('card', {style: style});
        cardElement.mount('#card-element');
        cardElement.on('change', cardElementChangeHandler);

        form.addEventListener('submit', function(event) {
            // We don't want to let default form submission happen here,
            // which would refresh the page.
            event.preventDefault();
            $btn.prop('disabled', true);
        
            stripe.createPaymentMethod({
                type: 'card',
                card: cardElement,
                billing_details: {
                    // Include any additional collected billing details.
                    name: 'Jenny Rosen'
                },
            }).then(stripePaymentMethodHandler);
        });
    }

    
    function cardElementChangeHandler(event) {
        if(event.complete) {
            console.info("complete!");
            $btn.prop('disabled', false);
        } else {
            $btn.prop('disabled', true);
        }
    }
    
    function stripePaymentMethodHandler(result) {
        if (result.error) {
            // Show error in payment form
            showAlert('Payment failed! ' + result.error.message, 'alert-danger');
        } else {
            // Otherwise send paymentMethod.id to your server (see Step 4)
            fetch('/home/pay', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        payment_method_id: result.paymentMethod.id,
                    })
                }).then(function(result) {
                    // Handle server response (see Step 4)
                    result.json().then(function(json) {
                        handleServerResponse(json);
                    })
                });
        }
    
        $btn.prop('disabled', false);
    }
    
    function handleServerResponse(json) {
        console.info(JSON.stringify(json));
        showAlert('Payment succeeded!', 'alert-success');
    }
    
    function showAlert(text, className) {
        var $alert = $alertTemplate.clone();
        $alert.addClass(className);
        $alert.appendTo($alertPlaceHolder);
        $alert.prepend(text);
    }

    app.initCheckout = initCheckout;
})(app);
