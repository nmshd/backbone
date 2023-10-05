using Enmeshed.Tooling.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
public static class DbContextOptionsBuilderExtensions
{
    public static IServiceCollection AddSaveChangesTimeInterceptor(this IServiceCollection services)
    {
        if (EnvironmentVariables.DEBUG_PERFORMANCE)
            services.TryAddScoped<SaveChangesTimeInterceptor>();

        return services;
    }
}
