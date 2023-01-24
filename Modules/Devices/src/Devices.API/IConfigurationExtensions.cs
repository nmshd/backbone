using System.Diagnostics.CodeAnalysis;
using Devices.Infrastructure.EventBus;

namespace Devices.API;

[ExcludeFromCodeCoverage]
internal static class IConfigurationExtensions
{
    public static AuthenticationConfiguration GetAuthentication(this IConfiguration configuration)
    {
        return new AuthenticationConfiguration(configuration);
    }

    public static AzureNotificationHubsConfiguration GetAzureNotificationHubsConfiguration(this IConfiguration configuration)
    {
        return new AzureNotificationHubsConfiguration(configuration);
    }

    public static EventBusConfiguration GetEventBusConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("EventBus").Get<EventBusConfiguration>() ?? new EventBusConfiguration();
    }

    public static ApplicationInsightsConfiguration GetApplicationInsightsConfiguration(this IConfiguration configuration)
    {
        return new ApplicationInsightsConfiguration(configuration);
    }

    public static KeyVaultConfiguration GetKeyVaultConfiguration(this IConfiguration configuration)
    {
        return new KeyVaultConfiguration(configuration);
    }

    public static AzureAppConfigurationConfiguration GetAzureAppConfigurationConfiguration(this IConfiguration configuration)
    {
        return new AzureAppConfigurationConfiguration(configuration);
    }

    public static SqlDatabaseConfiguration GetSqlDatabaseConfiguration(this IConfiguration configuration)
    {
        return new SqlDatabaseConfiguration(configuration);
    }

    public static CorsConfiguration GetCorsConfiguration(this IConfiguration configuration)
    {
        return new CorsConfiguration(configuration);
    }

    public static JwtOptionsConfiguration GetJwtOptionsConfiguration(this IConfiguration configuration)
    {
        return new JwtOptionsConfiguration(configuration);
    }

    public class AzureNotificationHubsConfiguration
    {
        private readonly IConfigurationSection _azureAppConfigurationConfiguration;

        public AzureNotificationHubsConfiguration(IConfiguration configuration)
        {
            _azureAppConfigurationConfiguration = configuration.GetSection("AzureNotificationHubs");
        }

        public string ConnectionString => _azureAppConfigurationConfiguration["ConnectionString"] ?? "";
        public string HubName => _azureAppConfigurationConfiguration["HubName"] ?? "";
    }
}

public class AuthenticationConfiguration
{
    private readonly IConfigurationSection _authorizationConfiguration;

    public AuthenticationConfiguration(IConfiguration configuration)
    {
        _authorizationConfiguration = configuration.GetSection("AuthenticationConfiguration");
    }

    public string IssuerUri => _authorizationConfiguration["IssuerUri"] ?? "";
}

public class KeyVaultConfiguration
{
    private readonly IConfigurationSection _keyVaultConfiguration;

    public KeyVaultConfiguration(IConfiguration configuration)
    {
        _keyVaultConfiguration = configuration.GetSection("AzureKeyVault");
    }

    public string ClientId => _keyVaultConfiguration["ClientId"] ?? "";
    public string ClientSecret => _keyVaultConfiguration["ClientSecret"] ?? "";
}

public class AzureAppConfigurationConfiguration
{
    private readonly IConfigurationSection _azureAppConfigurationConfiguration;

    public AzureAppConfigurationConfiguration(IConfiguration configuration)
    {
        _azureAppConfigurationConfiguration = configuration.GetSection("AzureAppConfiguration");
    }

    public bool Enabled => !string.IsNullOrEmpty(ConnectionString + Endpoint);
    public string ConnectionString => _azureAppConfigurationConfiguration["ConnectionString"] ?? "";
    public string Endpoint => _azureAppConfigurationConfiguration["Endpoint"] ?? "";
}

public class SqlDatabaseConfiguration
{
    private readonly IConfigurationSection _sqlDatabaseConfiguration;

    public SqlDatabaseConfiguration(IConfiguration configuration)
    {
        _sqlDatabaseConfiguration = configuration.GetSection("SqlDatabase");
    }

    public string ConnectionString => _sqlDatabaseConfiguration["ConnectionString"] ?? "";
}

public class CorsConfiguration
{
    private readonly IConfigurationSection _corsConfiguration;

    public CorsConfiguration(IConfiguration configuration)
    {
        _corsConfiguration = configuration.GetSection("CORS");
    }

    public List<string> AllowedOrigins
    {
        get
        {
            var allowedOrigins = new List<string>();

            var allowedOriginsString = _corsConfiguration["AllowedOrigins"];

            if (allowedOriginsString != null)
                allowedOrigins.AddRange(allowedOriginsString.Split(";"));

            return allowedOrigins;
        }
    }

    public List<string> ExposedHeaders
    {
        get
        {
            var allowedOrigins = new List<string>();

            var allowedOriginsString = _corsConfiguration["ExposedHeaders"];

            if (allowedOriginsString != null)
                allowedOrigins.AddRange(allowedOriginsString.Split(";"));

            return allowedOrigins;
        }
    }
}

public class ApplicationInsightsConfiguration
{
    private readonly IConfigurationSection _applicationInsightsConfiguration;

    public ApplicationInsightsConfiguration(IConfiguration configuration)
    {
        _applicationInsightsConfiguration = configuration.GetSection("ApplicationInsights");
    }

    public bool Enabled => _applicationInsightsConfiguration.GetValue<bool>("Enabled");
}

public class JwtOptionsConfiguration
{
    public enum SigningCertificateSourceEnum
    {
        File,
        Config,
        Unknown
    }

    private readonly IConfigurationSection _jwtOptionsConfiguration;

    public JwtOptionsConfiguration(IConfiguration configuration)
    {
        _jwtOptionsConfiguration = configuration.GetSection("JwtOptions");
    }

    public SigningCertificateSourceEnum SigningCertificateSource
    {
        get
        {
            var sourceString = _jwtOptionsConfiguration.GetValue<string>("SigningCertificateSource");

            return sourceString switch
            {
                "file" => SigningCertificateSourceEnum.File,
                "config" => SigningCertificateSourceEnum.Config,
                _ => SigningCertificateSourceEnum.Unknown
            };
        }
    }

    public string SigningCertificate => _jwtOptionsConfiguration.GetValue<string>("SigningCertificate");
}
