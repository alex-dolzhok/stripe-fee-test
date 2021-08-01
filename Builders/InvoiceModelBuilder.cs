using System;
using System.Linq;
using CardIssuerCountry.Configuration;
using CardIssuerCountry.Classifiers;
using Microsoft.Extensions.Options;

namespace CardIssuerCountry.Builders
{
    public class InvoiceModelBuilder
    {
        private readonly ProductOption product;
        private readonly StripeFeeOptions stripeFeeOptions;

        public InvoiceModelBuilder(
            IOptions<StripeFeeOptions> stripeFeeOptions,
            IOptions<ProductOption> productOption)
        {
            this.stripeFeeOptions = stripeFeeOptions.Value;
            this.product = productOption.Value;
        }

        public InvoiceModel Build(string cardCountryCode)
        {
            var feeProvider = stripeFeeOptions.StripeFeeProviders.FirstOrDefault(x => 
                    x.CurrencyCode.Equals(product.Currency, StringComparison.InvariantCultureIgnoreCase))
                ?? throw new ApplicationException($"Unable to find stripe fee for currency {product.Currency}");
            
            var cardCountry = Countries.All.FirstOrDefault(x => 
                    x.CountryCodeA2.Equals(cardCountryCode, StringComparison.InvariantCultureIgnoreCase))
                ?? throw new ApplicationException($"Unable to find country by code {cardCountryCode}");

            var fee = feeProvider!.GetFee(cardCountry);

            return new InvoiceModel
            {
                Currency = product.Currency,
                Amount = product.Amount,
                TransactionFee = CalculateFinalInvoicePrice(product.Amount, fee) - product.Amount,
                CardCountry = cardCountry.Name
            };
        }

        public decimal CalculateFinalInvoicePrice(decimal amount, Fee fee) =>
            (amount + fee.FixedFee) / (1 - fee.PercentageFee / 100);
    }
}