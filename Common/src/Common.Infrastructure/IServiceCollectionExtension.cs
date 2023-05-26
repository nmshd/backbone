using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.Common.Infrastructure;
public static class IServiceCollectionExtension
{
    public static IServiceCollection AddCommonRepositories<TDbContext>(this IServiceCollection services) where TDbContext : AbstractDbContextBase
    {
        services.AddTransient<IMetricStatusesRepository, MetricStatusesRepository<TDbContext>>();

        return services;
    }
}
