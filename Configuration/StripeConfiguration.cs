using System;
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
            var stripeKeyOptions = configuration.GetSection("StripeOptions:KeyOptions");
            services.Configure<StripeKeyOptions>(stripeKeyOptions);

            var keyOptions = stripeKeyOptions.Get<StripeKeyOptions>();

            Stripe.StripeConfiguration.ApiKey = keyOptions?.ApiKey
                ?? throw new ApplicationException("Unable to find Stripe Api key");
            
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