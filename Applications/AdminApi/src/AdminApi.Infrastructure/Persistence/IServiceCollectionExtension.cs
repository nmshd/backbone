using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
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

    public static IServiceCollection AddDatabase(this IServiceCollection services, Action<DatabaseConfiguration> setupOptions)
    {
        var options = new DatabaseConfiguration();
        setupOptions.Invoke(options);

        services
            .AddDbContext<AdminApiDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "AdminUi");
                        });
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "AdminUi");
                        });
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
            });

        return services;
    }
}
