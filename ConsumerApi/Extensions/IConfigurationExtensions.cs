using System.Diagnostics.CodeAnalysis;

namespace ConsumerApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class IConfigurationExtensions
{
    public static ApplicationInsightsConfiguration GetApplicationInsightsConfiguration(
        this IConfiguration configuration)
    {
        return configuration.GetSection("ApplicationInsights").Get<ApplicationInsightsConfiguration>() ??
               new ApplicationInsightsConfiguration();
    }
    
    public static AzureAppConfigurationConfiguration GetAzureAppConfigurationConfiguration(
        this IConfiguration configuration)
    {
        return configuration.GetSection("AzureAppConfiguration").Get<AzureAppConfigurationConfiguration>() ??
               new AzureAppConfigurationConfiguration();
    }
}

public class AzureAppConfigurationConfiguration
{
    public bool Enabled => !string.IsNullOrEmpty(ConnectionString + Endpoint);
    public string ConnectionString { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
}

public class ApplicationInsightsConfiguration
{
    public bool Enabled { get; set; }
}
