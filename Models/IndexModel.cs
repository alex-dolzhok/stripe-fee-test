using System;

namespace CardIssuerCountry
{
    public class IndexModel
    {
        public InvoiceModel Invoice { get; }

        public IndexModel(InvoiceModel invoice)
        {
            Invoice = invoice;
        }
    }
}