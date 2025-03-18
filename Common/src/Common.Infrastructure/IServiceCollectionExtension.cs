using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Common.Infrastructure.Persistence.Context;
using Backbone.Common.Infrastructure.Persistence.Repository;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Common.Infrastructure;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddMetricStatusesRepository(this IServiceCollection services, string provider, string connectionString)
    {
        services.Configure<MetricStatusesRepositoryOptions>(options => options.ConnectionString = connectionString);

        SqlMapper.AddTypeHandler(new MetricKeyTypeHandler());

        switch (provider)
        {
            case IServiceCollectionExtensions.SQLSERVER:
                services.AddTransient<IMetricStatusesRepository, SqlServerMetricStatusesRepository>();
                break;
            case IServiceCollectionExtensions.POSTGRES:
                services.AddTransient<IMetricStatusesRepository, PostgresMetricStatusesRepository>();
                break;
        }

        return services;
    }
}
