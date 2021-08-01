using CardIssuerCountry;

namespace CardIssuerCountry.Repositories
{
    public interface IStripeFeeProvider
    {
        string CurrencyCode { get; }
		Fee GetFee(Country country);
    }
}