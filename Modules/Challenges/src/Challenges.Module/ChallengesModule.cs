using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Challenges.Application;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Infrastructure;
using Backbone.Modules.Challenges.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Challenges.Module;

public class ChallengesModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Challenges";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfigurationConfiguration, IConfigurationSection _)
    {
        services.AddApplication();

        services.AddDatabase(dbOptions =>
        {
            dbOptions.Provider = infrastructureConfigurationConfiguration.SqlDatabase.Provider;
            dbOptions.ConnectionString = infrastructureConfigurationConfiguration.SqlDatabase.ConnectionString;
        });

        if (infrastructureConfigurationConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfigurationConfiguration.SqlDatabase.Provider, infrastructureConfigurationConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
