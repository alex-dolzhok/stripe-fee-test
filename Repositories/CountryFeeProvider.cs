using CardIssuerCountry;

namespace CardIssuerCountry.Repositories
{
    public class CountryFeeProvider : IStripeFeeProvider
	{
		private readonly CreditCardFee stripeFee;
		public string CurrencyCode => stripeFee.Currency;

		public CountryFeeProvider(CreditCardFee multiCountryFee) =>
			stripeFee = multiCountryFee;

		public Fee GetFee(Country country)
		{
			if (stripeFee.SingleCountryCode != null)
			{
				return stripeFee.SingleCountryCode.Equals(country.CountryCodeA2, System.StringComparison.OrdinalIgnoreCase)
					? stripeFee.LocalCardFee
					: stripeFee.InternationalCardFee;
			}

			return country.IsEuCountry ? stripeFee.LocalCardFee : stripeFee.InternationalCardFee;
		}
	}
}