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
                            dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(20);
                                sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            });
                            break;
                        case POSTGRES:
                            dbContextOptions.UseNpgsql(options.DbConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(20);
                                sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                                sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Tokens"); //TODO: Remove this once the issue with package 'Npgsql.EntityFrameworkCore.PostgreSQL' is fixed https://github.com/npgsql/efcore.pg/issues/2878
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
    public string Provider { get; set; } = null!;
    public string DbConnectionString { get; set; } = null!;
    public RetryOptions RetryOptions { get; set; } = new();
}

public class RetryOptions
{
    public byte MaxRetryCount { get; set; } = 15;
    public int MaxRetryDelayInSeconds { get; set; } = 30;
}
