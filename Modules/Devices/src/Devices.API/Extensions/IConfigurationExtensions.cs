using System.Diagnostics.CodeAnalysis;

namespace Devices.API.Extensions;

[ExcludeFromCodeCoverage]
internal static class IConfigurationExtensions
{
    public static AuthenticationConfiguration GetAuthorizationConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("Authentication").Get<AuthenticationConfiguration>() ?? new AuthenticationConfiguration();
    }

    public static SqlDatabaseConfiguration GetSqlDatabaseConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("SqlDatabase").Get<SqlDatabaseConfiguration>() ?? new SqlDatabaseConfiguration();
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

    public List<string> AllowedOrigins => _corsConfiguration["AllowedOrigins"] != null ? _corsConfiguration["AllowedOrigins"].Split(";").ToList() : new List<string>();
}
