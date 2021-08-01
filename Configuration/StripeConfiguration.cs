using System.Linq;
using CardIssuerCountry;
using CardIssuerCountry.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CardIssuerCountry.Configuration
{
    public class StripeConfiguration
    {
        private readonly IConfiguration configuration;

        public StripeConfiguration(IConfiguration configuration) =>
            this.configuration = configuration;

        public void ConfigureServiceLevel(IServiceCollection services)
        {
            Stripe.StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeOptins:ApiKey");
            
            services.Configure<StripeFeeOptions>(options =>
                options.StripeFeeProviders = configuration.GetSection("StripeOptions:FeeOptions")
                    .GetChildren()
                    .Select(GetFeeProvider)
                    .ToList());
        }

        private static IStripeFeeProvider GetFeeProvider(IConfigurationSection configurationSection) =>
            new CountryFeeProvider(configurationSection.Get<CreditCardFee>());
    }
}