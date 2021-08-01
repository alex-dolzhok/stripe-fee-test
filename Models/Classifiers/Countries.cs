using System.Collections.Generic;
using System.Linq;

namespace CardIssuerCountry.Classifiers
{
    public static class Countries
    {
        private static IEnumerable<Country> all;
        public static IReadOnlyList<Country> All => all.ToList();

        static Countries()
        {
            all = new Country[]
            {
                new Country("United States", "us", false),
                new Country("Switzerland", "ch", false),
                new Country("United Kingdom", "gb", false),
                new Country("Denmark", "dk", true),
                new Country("Sweden", "se", true),
                new Country("Norway", "no", true)
            };
        }
    }
}