using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services, BlobStorageOptions blobStorageOptions)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddTransient<ITokenRepository, TokenRepository>();
        services.Configure<TokenRepositoryOptions>(options => options.BlobRootFolder = blobStorageOptions.Container);
    }
}
