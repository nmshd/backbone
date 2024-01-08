using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Application.Extensions;
using Backbone.Modules.Tokens.Infrastructure.Persistence;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tokens.ConsumerApi;

public class TokensModule : AbstractModule
{
    public override string Name => "Tokens";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            options.DbOptions.ConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;

            if (parsedConfiguration.Infrastructure.BlobStorage != null)
            {
                options.BlobStorageOptions = new()
                {
                    CloudProvider = parsedConfiguration.Infrastructure.BlobStorage.CloudProvider,
                    ConnectionInfo = parsedConfiguration.Infrastructure.BlobStorage.ConnectionInfo,
                    Container = parsedConfiguration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                        ? "tokens"
                        : parsedConfiguration.Infrastructure.BlobStorage.ContainerName
                };
            }
        });

        services.AddApplication();

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
    }
}
