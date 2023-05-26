using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Enmeshed.Common.Infrastructure.Persistence.Context;

namespace Enmeshed.Common.Infrastructure;
public static class IServiceCollectionExtension
{
    public static IServiceCollection AddCommonRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMetricStatusesRepository, MetricStatusesRepository>();
        services.AddSingleton(new DapperContext(configuration));

        return services;
    }
}
