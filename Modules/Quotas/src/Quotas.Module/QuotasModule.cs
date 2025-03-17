using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Quotas.Application;
using Backbone.Modules.Quotas.Application.Extensions;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Module;

public class QuotasModule : AbstractModule<ApplicationOptions, InfrastructureConfiguration>
{
    public override string Name => "Quotas";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddApplication();

        services.AddDatabase(dbOptions =>
        {
            dbOptions.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            dbOptions.DbConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);

        services.AddResponseCaching();
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddQuotasDomainEventSubscriptions();
    }
}
