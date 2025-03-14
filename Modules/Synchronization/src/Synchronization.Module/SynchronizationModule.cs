using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Synchronization.Application;
using Backbone.Modules.Synchronization.Application.Extensions;
using Backbone.Modules.Synchronization.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Synchronization.Module;

public class SynchronizationModule : AbstractModule<ApplicationOptions, Configuration.InfrastructureConfiguration>
{
    public override string Name => "Synchronization";

    protected override void ConfigureServices(IServiceCollection services, Configuration.InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        services.AddApplication();

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddSynchronizationDomainEventSubscriptions();
    }
}
