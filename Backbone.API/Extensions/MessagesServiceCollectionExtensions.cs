using Backbone.API.Configuration;
using Challenges.Application.Extensions;
using Challenges.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Relationships.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;


namespace Backbone.API.Extensions;

public static class MessagesServiceCollectionExtensions
{
    public static IServiceCollection AddMessages(this IServiceCollection services,
        MessagesConfiguration configuration)
    {
        services.AddPersistence(options =>
        {
            options.DbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions.CloudProvider = configuration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.ConnectionInfo = configuration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.Container =
                configuration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "messages"
                    : configuration.Infrastructure.BlobStorage.ContainerName;
        });

        services.AddApplication();

        return services;
    }
}