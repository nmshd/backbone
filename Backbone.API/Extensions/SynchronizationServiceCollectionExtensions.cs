using Backbone.API.Configuration;
using Backbone.Modules.Synchronization.Application.Extensions;
using Backbone.Modules.Synchronization.Infrastructure.Persistence;
using Enmeshed.Tooling.Extensions;


namespace Backbone.API.Extensions;

public static class SynchronizationServiceCollectionExtensions
{
    public static IServiceCollection AddSynchronization(this IServiceCollection services,
        SynchronizationConfiguration configuration)
    {
        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = configuration.Infrastructure.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions.CloudProvider = configuration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.ConnectionInfo = configuration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.Container =
                configuration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "synchronization"
                    : configuration.Infrastructure.BlobStorage.ContainerName;
        });

        services.AddApplication();

        return services;
    }
}