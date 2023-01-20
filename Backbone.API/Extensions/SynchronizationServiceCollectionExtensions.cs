using Backbone.API.Configuration;
using Microsoft.IdentityModel.Tokens;
using Synchronization.Application.Extensions;
using Synchronization.Infrastructure.Persistence;


namespace Backbone.API.Extensions;

public static class SynchronizationServiceCollectionExtensions
{
    public static IServiceCollection AddSynchronization(this IServiceCollection services,
        SynchronizationConfiguration configuration)
    {
        services.AddPersistence(options =>
        {
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