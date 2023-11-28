using Backbone.AdminUi.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
//using PostgresModel = AdminUi.Infrastructure.CompiledModels.Postgres; TODO: Add this when issues with PostgreSQL compiled models are fixed https://github.com/npgsql/efcore.pg/issues/2972
using SqlServerModel = AdminUi.Infrastructure.CompiledModels.SqlServer;

namespace Backbone.AdminUi.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.AdminUi.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.AdminUi.Infrastructure.Database.Postgres";

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
        setupOptions?.Invoke(options);

        services
            .AddDbContext<AdminUiDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                        }).UseModel(SqlServerModel.AdminUiDbContextModel.Instance);
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                                "AdminUi"); //TODO: Remove this once the issue with package 'Npgsql.EntityFrameworkCore.PostgreSQL' is fixed https://github.com/npgsql/efcore.pg/issues/2878
                        });//.UseModel(PostgresModel.AdminUiDbContextModel.Instance); TODO: Add this when issues with PostgreSQL compiled models are fixed
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
            });

        return services;
    }

    public class DbOptions
    {
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public RetryOptions RetryOptions { get; set; } = new();
    }

    public class RetryOptions
    {
        public byte MaxRetryCount { get; set; } = 15;
        public int MaxRetryDelayInSeconds { get; set; } = 30;
    }
}
