using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace Backbone.ConsumerApi.Tests.Integration.Support;

[UsedImplicitly(Reason = "This is used by Reqnroll.")]
public static class Dependencies
{
    private const string APP_SETTINGS_FILE = "appsettings.json";

    [ScenarioDependencies]
    [UsedImplicitly(Reason = "This is used by Reqnroll.")]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), APP_SETTINGS_FILE), optional: false, reloadOnChange: false)
            .Build();

        services.ConfigureAndValidate<HttpConfiguration>(options => config.GetSection("Http").Bind(options));

        services.AddSingleton(new HttpClientFactory(new CustomWebApplicationFactory()));

        // For some reason the DI container is not able to use the internal constructor of the ClientPool. Hence we have to create it manually
        services.AddScoped<ClientPool>(sp => new ClientPool(sp.GetRequiredService<HttpClientFactory>(), sp.GetRequiredService<IOptions<HttpConfiguration>>()));

        services.AddScoped<ChallengesContext>();
        services.AddScoped<FilesContext>();
        services.AddScoped<IdentitiesContext>();
        services.AddScoped<MessagesContext>();
        services.AddScoped<RelationshipsContext>();
        services.AddScoped<RelationshipTemplatesContext>();
        services.AddScoped<ResponseContext>();
        services.AddScoped<TokensContext>();

        return services;
    }
}
