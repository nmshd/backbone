using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.Tokens.Application.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Tokens.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services, BlobStorageOptions blobStorageOptions)
    {
        services.AddTransient<ITokensRepository, TokensRepository>();
        services.Configure<TokensRepositoryOptions>(options => options.BlobRootFolder = blobStorageOptions.Container);
    }
}
