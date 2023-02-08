using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Messages.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Messages.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DbOptions options)
    {
        services.AddDbContext<MessagesDbContext>(dbContextOptions => _ = options.Provider switch
        {
            SQLSERVER => dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(20);
                sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
            }),

            POSTGRES => dbContextOptions.UseNpgsql(options.DbConnectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(20);
                sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
            }),

            _ => throw new Exception($"Unsupported database provider: {options.Provider}")
        });

        services.AddScoped<IMessagesDbContext, MessagesDbContext>();
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
