using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;
using Enmeshed.Common.Infrastructure.Persistence.Context;

namespace Enmeshed.Common.Infrastructure;
public static class IServiceCollectionExtension
{
    public static IServiceCollection AddMetricStatusesRepository(this IServiceCollection services, Action<MetricStatusesDapperContext> configureOptions)
    {
        services.AddTransient<IMetricStatusesRepository, MetricStatusesRepository>();
        services.Configure(configureOptions);
        services.AddTransient<MetricStatusesDapperContext>();

        return services;
    }
}
