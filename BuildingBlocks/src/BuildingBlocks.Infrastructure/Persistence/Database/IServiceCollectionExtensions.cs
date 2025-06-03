using Backbone.Tooling.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    public const string SQLSERVER = "SqlServer";
    public const string POSTGRES = "Postgres";

    public static IServiceCollection AddSaveChangesTimeInterceptor(this IServiceCollection services)
    {
        if (EnvironmentVariables.DEBUG_PERFORMANCE)
            services.TryAddScoped<SaveChangesTimeInterceptor>();

        return services;
    }

    public static void AddDbContextForModule<T>(this IServiceCollection services, DatabaseConfiguration configuration, string moduleName, Action<DbContextOptionsBuilder>? setupDbContextOptions = null)
        where T : DbContext
    {
        const string sqlserverMigrationsAssembly = "Backbone.Modules.<module_name>.Infrastructure.Database.SqlServer";
        const string postgresMigrationsAssembly = "Backbone.Modules.<module_name>.Infrastructure.Database.Postgres";

        AddDbContext<T>(services, configuration,
            p =>
            {
                var migrationsAssemblyNameTemplate = p switch
                {
                    SQLSERVER => sqlserverMigrationsAssembly,
                    POSTGRES => postgresMigrationsAssembly,
                    _ => throw new Exception($"Unsupported database provider for module {moduleName}")
                };

                return migrationsAssemblyNameTemplate.Replace("<module_name>", moduleName);
            }, moduleName, setupDbContextOptions);
    }

    public static void AddDbContext<T>(this IServiceCollection services, DatabaseConfiguration configuration, Func<string, string> migrationAssemblyNameBuilder, string schemaName,
        Action<DbContextOptionsBuilder>? setupDbContextOptions = null, QueryTrackingBehavior queryTrackingBehavior = QueryTrackingBehavior.TrackAll) where T : DbContext
    {
        var migrationsAssemblyName = migrationAssemblyNameBuilder(configuration.Provider);

        services.AddDbContext<T>(dbContextOptions =>
        {
            dbContextOptions.UseQueryTrackingBehavior(queryTrackingBehavior);

            switch (configuration.Provider)
            {
                case SQLSERVER:
                    dbContextOptions.UseSqlServer(configuration.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(configuration.CommandTimeout);
                        sqlOptions.MigrationsAssembly(migrationsAssemblyName);
                        sqlOptions.EnableRetryOnFailure(configuration.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(configuration.RetryConfiguration.MaxRetryDelayInSeconds), null);
                        sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schemaName);
                    });
                    break;
                case POSTGRES:
                    dbContextOptions.UseNpgsql(configuration.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(configuration.CommandTimeout);
                        sqlOptions.MigrationsAssembly(migrationsAssemblyName);
                        sqlOptions.EnableRetryOnFailure(configuration.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(configuration.RetryConfiguration.MaxRetryDelayInSeconds), null);
                        sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schemaName);
                    });
                    break;
                default:
                    throw new Exception($"'{configuration.Provider}' is not a supported database provider.");
            }

            setupDbContextOptions?.Invoke(dbContextOptions);
        });
    }
}
