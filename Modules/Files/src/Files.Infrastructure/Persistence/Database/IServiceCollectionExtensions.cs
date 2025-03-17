using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Files.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Files.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DatabaseConfiguration> setupOptions)
    {
        var options = new DatabaseConfiguration();
        setupOptions.Invoke(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration options)
    {
        services
            .AddDbContext<FilesDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Files");
                        });
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Files");
                        });
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
            });

        services.AddScoped<IFilesDbContext, FilesDbContext>();
    }
}
