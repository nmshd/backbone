using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Application.Extensions;
using Backbone.Modules.Tokens.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.ConsumerApi;

public class TokensModule : AbstractModule<ApplicationOptions, Configuration.InfrastructureConfiguration>
{
    public override string Name => "Tokens";

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

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
