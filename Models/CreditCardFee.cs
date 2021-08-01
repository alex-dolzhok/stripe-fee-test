namespace CardIssuerCountry
{
    public class CreditCardFee
    {
        public string Currency { get; set; } = null!;
		public string? SingleCountryCode { get; set; }
		public Fee LocalCardFee { get; set; }
		public Fee InternationalCardFee { get; set; }
    }
}