using Backbone.Common.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Repository;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure;
public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string POSTGRES = "Postgres";

    public static IServiceCollection AddConsistencyCheckRepository(this IServiceCollection services, string provider, string connectionString)
    {
        services.Configure<DapperRepositoryOptions>(options => options.ConnectionString = connectionString);

        switch (provider)
        {
            case SQLSERVER:
                services.AddTransient<IConsistencyCheckRepository, SqlServerConsistencyCheckRepository>();
                break;
            case POSTGRES:
                services.AddTransient<IConsistencyCheckRepository, PostgresConsistencyCheckRepository>();
                break;
        }

        return services;
    }
}
