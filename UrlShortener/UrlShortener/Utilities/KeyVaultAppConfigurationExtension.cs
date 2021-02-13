using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace UrlShortener.Utilities
{
    public static class KeyVaultAppConfigurationExtension
    {
        public static IConfigurationBuilder InitKeyVault(this IConfigurationBuilder builder)
        {
            var keyVaultEndpoint = Environment.GetEnvironmentVariable(Constants.ConfigKeyVaultEndpoint);

            if (string.IsNullOrWhiteSpace(keyVaultEndpoint))
                throw new InvalidOperationException(
                    $"Key vault environment variable {Constants.ConfigKeyVaultEndpoint} is not set");

            builder.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
            return builder;
        }
    }
}