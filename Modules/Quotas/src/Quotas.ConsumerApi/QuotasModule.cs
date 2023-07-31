using Backbone.Modules.Quotas.Application;
using Backbone.Modules.Quotas.Application.Extensions;
using Backbone.Modules.Quotas.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Quotas.ConsumerApi;

public class QuotasModule : IModule
{
    public string Name => "Quotas";

    public void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddApplication();

        services.AddDatabase(dbOptions =>
        {
            dbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            dbOptions.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
        });

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public void ConfigureEventBus(IEventBus eventBus)
    {
        eventBus.AddQuotasIntegrationEventSubscriptions();
    }
}
