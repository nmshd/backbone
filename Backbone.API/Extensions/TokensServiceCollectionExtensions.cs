using Backbone.API.Configuration;
using Backbone.Modules.Tokens.Application.Extensions;
using Backbone.Modules.Tokens.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;

namespace Backbone.API.Extensions;

public static class TokensServiceCollectionExtensions
{
    public static IServiceCollection AddTokens(this IServiceCollection services,
        TokensConfiguration configuration)
    {
        services.AddPersistence(options =>
        {
            options.DbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions.CloudProvider = configuration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.ConnectionInfo = configuration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.Container =
                configuration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "tokens"
                    : configuration.Infrastructure.BlobStorage.ContainerName;
        });

        services.AddApplication();

        return services;
    }
}