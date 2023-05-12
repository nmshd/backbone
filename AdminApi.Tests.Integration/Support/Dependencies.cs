using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestSharp;
using SolidToken.SpecFlow.DependencyInjection;

namespace AdminApi.Tests.Integration.Support;

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

        services.ConfigureAndValidate<HttpConfiguration>(options => config.GetSection("Http").Bind(options));

        var serviceProvider = services.BuildServiceProvider();
        var httpConfig = serviceProvider.GetRequiredService<IOptions<HttpConfiguration>>().Value;

        var restClient = new RestClient(httpConfig.BaseUrl);

        var identitiesApi = new IdentitiesApi(restClient);
        var tiersApi = new TiersApi(restClient);

        services.AddSingleton(identitiesApi);
        services.AddSingleton(tiersApi);

        return services;
    }
}
