using System;
using CardIssuerCountry;

namespace CardIssuerCountry.Configuration
{
    public class ProductOption
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; } = null!;
    }
}