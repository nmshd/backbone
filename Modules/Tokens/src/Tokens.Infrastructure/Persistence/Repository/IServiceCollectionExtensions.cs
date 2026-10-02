using Backbone.Modules.Tokens.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ITokensRepository, TokensRepository>();
    }
}
