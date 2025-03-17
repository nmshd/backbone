using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Tokens.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Tokens.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions.Invoke(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DbOptions options)
    {
        services
            .AddDbContext<TokensDbContext>(dbContextOptions =>
                {
                    switch (options.Provider)
                    {
                        case SQLSERVER:
                            dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(options.CommandTimeout);
                                sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                                sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Tokens");
                            });
                            break;
                        case POSTGRES:
                            dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(options.CommandTimeout);
                                sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                                sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Tokens");
                            });
                            break;
                        default:
                            throw new Exception($"Unsupported database provider: {options.Provider}");
                    }
                }
            );
    }
}

public class DbOptions
{
    [Required]
    [MinLength(1)]
    [RegularExpression("SqlServer|Postgres")]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public bool EnableHealthCheck { get; set; } = true;

    public int CommandTimeout { get; set; } = 20;

    public RetryOptions RetryOptions { get; set; } = new();
}

public class RetryOptions
{
    public byte MaxRetryCount { get; set; } = 15;
    public int MaxRetryDelayInSeconds { get; set; } = 30;
}
