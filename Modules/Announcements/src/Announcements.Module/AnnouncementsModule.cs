using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Announcements.Application;
using Backbone.Modules.Announcements.Application.Extensions;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Announcements.Module;

public class AnnouncementsModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Announcements";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddApplication();

        services.AddDatabase(infrastructureConfiguration.SqlDatabase);

        // TODO: könnte auch in Oberklasse ausgelagert werden (dafür müsste es aber ein Interface geben, das jede InfrastructureConfiguration implementiert, und was die SqlDatabase Property enthält
        // Einziges Problem: Tags hat keine SqlDatabase. Könnte evtl. durch einen Overload dieser Methode gelöst werden
        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
