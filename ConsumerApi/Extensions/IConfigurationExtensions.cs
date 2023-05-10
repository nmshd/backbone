using System.Diagnostics.CodeAnalysis;

namespace ConsumerApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class IConfigurationExtensions
{
    public static AuthenticationConfiguration GetAuthorizationConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("Authentication").Get<AuthenticationConfiguration>() ??
               new AuthenticationConfiguration();
    }

    public static ApplicationInsightsConfiguration GetApplicationInsightsConfiguration(
        this IConfiguration configuration)
    {
        return configuration.GetSection("ApplicationInsights").Get<ApplicationInsightsConfiguration>() ??
               new ApplicationInsightsConfiguration();
    }

    public static KeyVaultConfiguration GetKeyVaultConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("AzureKeyVault").Get<KeyVaultConfiguration>() ?? new KeyVaultConfiguration();
    }

    public static AzureAppConfigurationConfiguration GetAzureAppConfigurationConfiguration(
        this IConfiguration configuration)
    {
        return configuration.GetSection("AzureAppConfiguration").Get<AzureAppConfigurationConfiguration>() ??
               new AzureAppConfigurationConfiguration();
    }

    public static CorsConfiguration GetCorsConfiguration(this IConfiguration configuration)
    {
        return new CorsConfiguration(configuration);
    }
}

public class AuthenticationConfiguration
{
    public string Authority { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string JwtSigningCertificate { get; set; } = string.Empty;
}

public class KeyVaultConfiguration
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class AzureAppConfigurationConfiguration
{
    public bool Enabled => !string.IsNullOrEmpty(ConnectionString + Endpoint);
    public string ConnectionString { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
}

public class CorsConfiguration
{
    private readonly IConfigurationSection _corsConfiguration;

    public CorsConfiguration(IConfiguration configuration)
    {
        _corsConfiguration = configuration.GetSection("CORS");
    }

    public List<string> AllowedOrigins => _corsConfiguration["AllowedOrigins"]?.Split(";").ToList();
    public List<string> ExposedHeaders => _corsConfiguration["ExposedHeaders"]?.Split(";").ToList();
}

public class ApplicationInsightsConfiguration
{
    public bool Enabled { get; set; }
}