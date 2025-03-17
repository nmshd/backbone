using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Relationships.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Relationships.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DatabaseConfiguration> setupOptions)
    {
        var options = new DatabaseConfiguration();
        setupOptions.Invoke(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration options)
    {
        services
            .AddDbContext<RelationshipsDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Relationships");
                        });
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Relationships");
                        });
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
            });
    }
}
