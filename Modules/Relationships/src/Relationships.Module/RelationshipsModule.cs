using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Relationships.Module;

public class RelationshipsModule : AbstractModule<ApplicationOptions, Configuration.InfrastructureConfiguration>
{
    public override string Name => "Relationships";

    protected override void ConfigureServices(IServiceCollection services, Configuration.InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddApplication();

        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddRelationshipsDomainEventSubscriptions();
    }
}
