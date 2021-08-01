using System;

namespace CardIssuerCountry
{
    public class IndexModel
    {
        public InvoiceModel Invoice { get; }
        public string StripePublishableKey { get; }

        public IndexModel(InvoiceModel invoice, string stripePublishableKey)
        {
            Invoice = invoice;
            StripePublishableKey = stripePublishableKey;
        }
    }
}