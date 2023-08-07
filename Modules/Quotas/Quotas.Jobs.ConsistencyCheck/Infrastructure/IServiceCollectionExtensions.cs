using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure;
public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string POSTGRES = "Postgres";

    public static void AddJobDatabases(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        switch (options.Provider)
        {
            case SQLSERVER:
                services.AddDbContext<DevicesDbContext>(dbContextOptions =>
                    dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(20);
                        sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                    })
                );
                break;
            case POSTGRES:
                services.AddDbContext<DevicesDbContext>(dbContextOptions =>
                    dbContextOptions.UseNpgsql(options.DbConnectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(20);
                        sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                    })

                );
                break;
            default:
                throw new Exception($"Unsupported database provider: {options.Provider}");
        }
    }

    public class DbOptions
    {
        public string Provider { get; set; }
        public string DbConnectionString { get; set; }
        public RetryOptions RetryOptions { get; set; } = new();
    }

    public class RetryOptions
    {
        public byte MaxRetryCount { get; set; } = 15;
        public int MaxRetryDelayInSeconds { get; set; } = 30;
    }
}
