using Enmeshed.Tooling.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
