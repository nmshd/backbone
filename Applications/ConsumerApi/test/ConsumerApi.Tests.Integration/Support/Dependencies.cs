using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.StepDefinitions;
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

        services.AddSingleton(new HttpClientFactory(new CustomWebApplicationFactory()));

        services.AddScoped<ChallengesContext>();
        services.AddScoped<DatawalletContext>();
        services.AddScoped<IdentitiesContext>();
        services.AddScoped<MessagesContext>();
        services.AddScoped<RelationshipsContext>();
        services.AddScoped<ResponseContext>();
        services.AddScoped<TokensContext>();

        return services;
    }
}
