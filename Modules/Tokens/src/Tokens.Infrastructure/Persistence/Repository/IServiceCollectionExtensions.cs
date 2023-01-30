using Backbone.Modules.Tokens.Application.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddTransient<ITokenRepository, TokenRepository>();
    }
}
