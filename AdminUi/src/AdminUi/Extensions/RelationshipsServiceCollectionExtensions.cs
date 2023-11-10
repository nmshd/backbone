using AdminUi.Configuration;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Infrastructure.Persistence;
using Enmeshed.Tooling.Extensions;

namespace AdminUi.Extensions;

public static class RelationshipsServiceCollectionExtensions
{
    public static IServiceCollection AddRelationships(this IServiceCollection services,
        RelationshipsConfiguration configuration)
    {
        services.AddApplication();

        services.AddPersistence(options =>
        {
            options.DbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
            options.DbOptions.Provider = configuration.Infrastructure.SqlDatabase.Provider;

            options.BlobStorageOptions.CloudProvider = configuration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.ConnectionInfo = configuration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.Container = configuration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                ? "relationships"
                : configuration.Infrastructure.BlobStorage.ContainerName;
        });


        return services;
    }
}
