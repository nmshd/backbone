using Backbone.Modules.Tokens.Application.Infrastructure;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services, BlobStorageOptions blobStorageOptions)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddTransient<ITokenRepository, TokenRepository>();
        services.Configure<TokenRepositoryOptions>(options => options.BlobRootFolder = blobStorageOptions.Container);
    }
}
