using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Application.Extensions;
using Backbone.Modules.Tokens.Infrastructure;
using Backbone.Modules.Tokens.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Module;

public class TokensModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Tokens";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddPersistence(infrastructureConfiguration.SqlDatabase);

        services.AddApplication();

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
