namespace CardIssuerCountry.Utils
{
    public static class Formatter
    {
        public static string Amount(decimal amount) => 
            amount.ToString("0.##");
    }
}