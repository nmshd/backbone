using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Enmeshed.Common.Infrastructure.Persistence.Context;

namespace Enmeshed.Common.Infrastructure;
public static class IServiceCollectionExtension
{
    public static IServiceCollection AddMetricStatusesRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMetricStatusesRepository, MetricStatusesRepository>();

        services.Configure<MetricStatusesDapperContext>(c => {
            c.ConnectionString = configuration.GetSection("Modules:Quotas:Infrastructure:SqlDatabase:ConnectionString").Value 
            ?? throw new Exception("Could not parse connection string for MetricStatuses.");
        });

        services.AddTransient<MetricStatusesDapperContext>();

        return services;
    }
}
