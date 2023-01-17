using System.Diagnostics.CodeAnalysis;

namespace Backbone.API.Extensions;

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

    public static SqlDatabaseConfiguration GetSqlDatabaseConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("SqlDatabase").Get<SqlDatabaseConfiguration>() ??
               new SqlDatabaseConfiguration();
    }

    public static CorsConfiguration GetCorsConfiguration(this IConfiguration configuration)
    {
        return new CorsConfiguration(configuration);
    }
}

public class AuthenticationConfiguration
{
    public string Authority { get; set; }
    public string ValidIssuer { get; set; }
    public string JwtSigningCertificate { get; set; }
}

public class KeyVaultConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public class AzureAppConfigurationConfiguration
{
    public bool Enabled => !string.IsNullOrEmpty(ConnectionString + Endpoint);
    public string ConnectionString { get; set; }
    public string Endpoint { get; set; }
}

public class SqlDatabaseConfiguration
{
    public string ConnectionString { get; set; }
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