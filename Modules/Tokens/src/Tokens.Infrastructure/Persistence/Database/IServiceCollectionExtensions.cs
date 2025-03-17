using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration options)
    {
        services.AddDbContextForModule<TokensDbContext>(options, "Tokens");
    }
}
