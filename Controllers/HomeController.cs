using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using CardIssuerCountry.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using CardIssuerCountry.Configuration;

namespace CardIssuerCountry.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Index(
            [FromServices] InvoiceModelBuilder invoiceModelBuilder,
            [FromServices] IOptions<StripeKeyOptions> stripeKeyOptions)
        {
            var stripePublishableKey = stripeKeyOptions.Value.PublishableKey;
            var invoiceModel = invoiceModelBuilder.Build("US");
            var model = new IndexModel(invoiceModel, stripePublishableKey);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Invoice([FromBody] ConfirmPaymentRequest request)
        {
            // TODO: return invoice model with invoice amount & fee based on card country
            return Ok();
        }

        public IActionResult Pay([FromBody] ConfirmPaymentRequest request)
        {
            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = null!;

            try
            {
                // var metadata = new Dictionary<string, string>();
                // metadata.Add("Id", "12345");

                // var options = new CustomerCreateOptions
                // {
                //     Email = "test@customer.com",
                //     Metadata = metadata
                // };

                //var service = new CustomerService();
                //var customer = service.Create(options);

                if (request.PaymentMethodId != null)
                {
                    var service = new PaymentMethodService();
                    var paymentMethod = service.Get(request.PaymentMethodId);
                    var country = paymentMethod.Card.Country;

                    // Create the PaymentIntent
                    var createOptions = new PaymentIntentCreateOptions
                    {
                        PaymentMethod = request.PaymentMethodId,
                        Amount = 1099,
                        Currency = "usd",
                        ConfirmationMethod = "manual",
                        Confirm = true,
                    };
                    paymentIntent = paymentIntentService.Create(createOptions);
                    var customer = paymentIntent.Customer;

                }
                if (request.PaymentIntentId != null)
                {
                    var confirmOptions = new PaymentIntentConfirmOptions { };
                    paymentIntent = paymentIntentService.Confirm(
                        request.PaymentIntentId,
                        confirmOptions
                    );
                }
            }
            catch (StripeException e)
            {
                return Json(new { error = e.StripeError.Message });
            }

            return GeneratePaymentResponse(paymentIntent);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult GeneratePaymentResponse(PaymentIntent intent)
        {
            // Note that if your API version is before 2019-02-11, 'requires_action'
            // appears as 'requires_source_action'.
            if (intent.Status == "requires_action" &&
                intent.NextAction.Type == "use_stripe_sdk")
            {
                // Tell the client to handle the action
                return Json(new
                {
                    requires_action = true,
                    payment_intent_client_secret = intent.ClientSecret
                });
            }
            else if (intent.Status == "succeeded")
            {
                // The payment didn’t need any additional actions and completed!
                // Handle post-payment fulfillment
                return Json(new { success = true });
            }
            else
            {
                // Invalid status
                return StatusCode(500, new { error = "Invalid PaymentIntent status" });
            }
        }
    }
}
