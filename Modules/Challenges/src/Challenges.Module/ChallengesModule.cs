using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Challenges.Application;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Challenges.Module;

public class ChallengesModule : AbstractModule<ApplicationConfiguration, ChallengesInfrastructure>
{
    public override string Name => "Challenges";

    protected override void ConfigureServices(IServiceCollection services, ChallengesInfrastructure infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddApplication();

        services.AddDatabase(dbOptions =>
        {
            dbOptions.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            dbOptions.DbConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
