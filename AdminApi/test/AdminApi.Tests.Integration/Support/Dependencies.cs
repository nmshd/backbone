using Backbone.AdminApi.Tests.Integration.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;

namespace Backbone.AdminApi.Tests.Integration.Support;

public static class Dependencies
{
    private const string APP_SETTINGS_FILE = "appsettings.json";

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), APP_SETTINGS_FILE), false, false)
            .Build();

        services.ConfigureAndValidate<HttpClientOptions>(options =>
            config.GetSection("AdminApi:Http").Bind(options)
        );

        services.AddSingleton(new HttpClientFactory(new CustomWebApplicationFactory<Program>()));
        services.AddSingleton<ISignatureHelper>(SignatureHelper.CreateEd25519WithRawKeyFormat());

        return services;
    }
}
