namespace CardIssuerCountry
{
    public class Country
    {
        public string Name { get; }
        public string CountryCodeA2 { get; }
        public bool IsEuCountry { get; }

        public Country(string name, string countryCodeA2, bool isEuCountry)
        {
            this.Name = name;
            this.CountryCodeA2 = countryCodeA2;
            IsEuCountry = isEuCountry;
        }
    }
}