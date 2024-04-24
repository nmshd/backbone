using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.AdminApi.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.AdminApi.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.AdminApi.Infrastructure.Database.Postgres";

    public static IServiceCollection AddDatabase(this IServiceCollection services, SqlDatabaseConfiguration configuration)
    {
        services.AddDatabase(options =>
        {
            options.ConnectionString = configuration.ConnectionString;
            options.Provider = configuration.Provider;
        });

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions.Invoke(options);

        services
            .AddDbContext<AdminApiDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "AdminUi");
                        });
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "AdminUi");
                        });
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
            });

        return services;
    }

    public class DbOptions
    {
        public string Provider { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;
        public RetryOptions RetryOptions { get; set; } = new();
    }

    public class RetryOptions
    {
        public byte MaxRetryCount { get; set; } = 15;
        public int MaxRetryDelayInSeconds { get; set; } = 30;
    }
}
