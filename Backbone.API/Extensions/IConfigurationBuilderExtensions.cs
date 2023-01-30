using Azure.Identity;

namespace Backbone.API.Extensions;

public static class IConfigurationBuilderExtensions
{
    public static void AddAzureAppConfiguration(this IConfigurationBuilder builder)
    {
        var configuration = builder.Build();

        var azureAppConfigurationConfiguration = configuration.GetAzureAppConfigurationConfiguration();

        if (azureAppConfigurationConfiguration.Enabled)
            builder.AddAzureAppConfiguration(appConfigurationOptions =>
            {
                var credentials = new ManagedIdentityCredential();

                appConfigurationOptions
                    .Connect(new Uri(azureAppConfigurationConfiguration.Endpoint), credentials)
                    .ConfigureKeyVault(vaultOptions => vaultOptions.SetCredential(credentials))
                    .Select("*");
            });
    }
}