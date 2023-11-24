using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;

namespace Backbone.ConsumerApi.Tests.Integration.Support;

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

        services.AddSingleton(new HttpClientFactory(new CustomWebApplicationFactory<Program>()));
        services.AddSingleton<ISignatureHelper>(SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddTransient<TokensApi>();
        services.AddTransient<ChallengesApi>();
        services.AddTransient<IdentitiesApi>();
        services.AddTransient<DevicesApi>();

        return services;
    }
}
