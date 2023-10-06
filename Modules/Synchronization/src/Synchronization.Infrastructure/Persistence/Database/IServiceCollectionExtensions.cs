using Backbone.Modules.Synchronization.Application.Infrastructure;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Enmeshed.Tooling.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Synchronization.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DbOptions options)
    {
        switch (options.Provider)
        {
            case SQLSERVER:
                services
                    .AddDbContext<SynchronizationDbContext>((provider, dbContextOptions) =>
                    {
                        dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                        });
                        dbContextOptions.AddInterceptors(provider.GetRequiredService<SaveChangesTimeInterceptor>());
                    });
                break;
            case POSTGRES:
                services
                    .AddDbContext<SynchronizationDbContext>((provider, dbContextOptions) =>
                    {
                        dbContextOptions.UseNpgsql(options.DbConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                        });
                        dbContextOptions.AddInterceptors(provider.GetRequiredService<SaveChangesTimeInterceptor>());
                    });
                break;
            default:
                throw new Exception($"Unsupported database provider: {options.Provider}");
        }

        services.AddScoped<ISynchronizationDbContext, SynchronizationDbContext>();
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
