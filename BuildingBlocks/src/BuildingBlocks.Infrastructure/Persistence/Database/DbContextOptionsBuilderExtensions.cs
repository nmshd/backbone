using Enmeshed.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
public static class DbContextOptionsBuilderExtensions
{
    public static IServiceCollection AddDebugPerformanceInterceptor(this IServiceCollection services)
    {
        if (EnvironmentVariables.DEBUG_PERFORMANCE)
            services.AddTransient<SaveChangesTimeInterceptor>();

        return services;
    }
}
