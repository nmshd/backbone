using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Challenges.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Challenges.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions.Invoke(options);

        services
            .AddDbContext<ChallengesDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Challenges");
                        });
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.DbConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Challenges");
                        });
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
            });

        services.AddScoped<IChallengesRepository, ChallengesRepository>();
    }

    public class DbOptions
    {
        public string Provider { get; set; } = null!;
        public string DbConnectionString { get; set; } = null!;
        public int CommandTimeout { get; set; } = 20;
        public RetryOptions RetryOptions { get; set; } = new();
    }

    public class RetryOptions
    {
        public byte MaxRetryCount { get; set; } = 15;
        public int MaxRetryDelayInSeconds { get; set; } = 30;
    }
}
