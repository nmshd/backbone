using Backbone.API.Configuration;
using Files.Application.Extensions;
using Files.Infrastructure.Persistence;
using Files.Infrastructure.Persistence.Database;
using Microsoft.IdentityModel.Tokens;

namespace Backbone.API.Extensions;

public static class FilesServiceCollectionExtensions
{
    public static IServiceCollection AddFiles(this IServiceCollection services,
        FilesConfiguration configuration)
    {
        services.AddApplication();

        services.AddPersistence(options =>
        {
            options.DbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions.ConnectionInfo = configuration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.CloudProvider = configuration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.Container =
                configuration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "files"
                    : configuration.Infrastructure.BlobStorage.ContainerName;
        });

        return services;
    }
}