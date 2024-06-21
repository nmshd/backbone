using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Files.Application;
using Backbone.Modules.Files.Application.Extensions;
using Backbone.Modules.Files.Infrastructure.Persistence;
using Backbone.Tooling.Extensions;
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

            options.BlobStorageOptions.ConnectionInfo = parsedConfiguration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.CloudProvider = parsedConfiguration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.Container =
                parsedConfiguration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "files"
                    : parsedConfiguration.Infrastructure.BlobStorage.ContainerName;

            if (options.BlobStorageOptions.IonosS3Config != null) options.BlobStorageOptions.IonosS3Config.AccessKey = parsedConfiguration.Infrastructure.BlobStorage.IonosS3Config.AccessKey;
            if (options.BlobStorageOptions.IonosS3Config != null) options.BlobStorageOptions.IonosS3Config.BucketName = parsedConfiguration.Infrastructure.BlobStorage.IonosS3Config.BucketName;
            if (options.BlobStorageOptions.IonosS3Config != null) options.BlobStorageOptions.IonosS3Config.SecretKey = parsedConfiguration.Infrastructure.BlobStorage.IonosS3Config.SecretKey;
            if (options.BlobStorageOptions.IonosS3Config != null) options.BlobStorageOptions.IonosS3Config.ServiceUrl = parsedConfiguration.Infrastructure.BlobStorage.IonosS3Config.ServiceUrl;
        });

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
    }
}
