using Backbone.API.Configuration;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;


namespace Backbone.API.Extensions;

public static class RelationshipsServiceCollectionExtensions
{
    public static IServiceCollection AddRelationships(this IServiceCollection services,
        RelationshipsConfiguration configuration)
    {
        services.AddPersistence(options =>
        {
            options.DbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions.CloudProvider = configuration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.ConnectionInfo = configuration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.Container =
                configuration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "relationships"
                    : configuration.Infrastructure.BlobStorage.ContainerName;
        });

        services.AddApplication();


        return services;
    }
}