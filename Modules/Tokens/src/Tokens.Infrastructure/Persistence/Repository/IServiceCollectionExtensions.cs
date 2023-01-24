using Microsoft.Extensions.DependencyInjection;
using Tokens.Application.Infrastructure;

namespace Tokens.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddTransient<ITokenRepository, TokenRepository>();
    }
}
