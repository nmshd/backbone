using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;

namespace AdminUi.Tests.Integration.Support;

public static class Dependencies
{
    private const string APP_SETTINGS_FILE = "appsettings.json";

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), APP_SETTINGS_FILE), optional: false, reloadOnChange: false)
            .Build();

        services.ConfigureAndValidate<HttpClientOptions>(options =>
            config.GetSection("AdminUi:Http").Bind(options)
        );

        services.AddSingleton(GetWebApplicationFactory());
        services.AddTransient<IdentitiesApi>();
        services.AddTransient<TiersApi>();
        services.AddTransient<ClientsApi>();
        services.AddTransient<MetricsApi>();
        services.AddTransient<LogsApi>();

        return services;
    }

    private static WebApplicationFactory<Program> GetWebApplicationFactory() => new();
}
