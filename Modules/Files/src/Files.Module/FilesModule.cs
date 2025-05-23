using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Files.Application;
using Backbone.Modules.Files.Application.Extensions;
using Backbone.Modules.Files.Infrastructure;
using Backbone.Modules.Files.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Module;

public class FilesModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Files";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddApplication();

        services.AddPersistence(infrastructureConfiguration.SqlDatabase, infrastructureConfiguration.BlobStorage);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
