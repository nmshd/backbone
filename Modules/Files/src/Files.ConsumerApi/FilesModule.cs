using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Files.Application;
using Backbone.Modules.Files.Application.Extensions;
using Backbone.Modules.Files.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Files.ConsumerApi;

public class FilesModule : AbstractModule
{
    public override string Name => "Files";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddApplication();

        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions = parsedConfiguration.Infrastructure.BlobStorage;
        });

        if (parsedConfiguration.Infrastructure.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name,
                parsedConfiguration.Infrastructure.SqlDatabase.Provider,
                parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
