using Backbone.Common.Infrastructure.Persistence.Context;
using Backbone.Common.Infrastructure.Persistence.Repository;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Common.Infrastructure;
public static class IServiceCollectionExtension
{
    private const string SQLSERVER = "SqlServer";
    private const string POSTGRES = "Postgres";

    public static IServiceCollection AddMetricStatusesRepository(this IServiceCollection services, string provider, string connectionString)
    {
        services.Configure<DapperRepositoryOptions>(options => options.ConnectionString = connectionString);

        SqlMapper.AddTypeHandler(new MetricKeyTypeHandler());

        switch (provider)
        {
            case SQLSERVER:
                services.AddTransient<IMetricStatusesRepository, SqlServerMetricStatusesRepository>();
                break;
            case POSTGRES:
                services.AddTransient<IMetricStatusesRepository, PostgresMetricStatusesRepository>();
                break;
        }

        return services;
    }
}
