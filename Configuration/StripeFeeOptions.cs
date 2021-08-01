using System.Collections.Generic;
using CardIssuerCountry.Repositories;

namespace CardIssuerCountry.Configuration
{
    public class StripeFeeOptions
    {
        public IReadOnlyList<IStripeFeeProvider> StripeFeeProviders { get; set; } = null!;
    }
}