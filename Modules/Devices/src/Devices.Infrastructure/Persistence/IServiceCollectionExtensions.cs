using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Devices.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Devices.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        switch (options.Provider)
        {
            case SQLSERVER:
                services.AddDbContext<DevicesDbContext>(dbContextOptions =>
                    dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(20);
                        sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                        sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                    }).UseOpenIddict()
                );
                break;
            case POSTGRES:
                services.AddDbContext<DevicesDbContext>(dbContextOptions =>
                    dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(20);
                        sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                        sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                    }).UseOpenIddict()

                );
                break;
            default:
                throw new Exception($"Unsupported database provider: {options.Provider}");

        }

        services.AddScoped<IDevicesDbContext, DevicesDbContext>();

        services.AddRepositories();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IIdentitiesRepository, IdentitiesRepository>();
        services.AddTransient<ITiersRepository, TiersRepository>();
        services.AddTransient<IChallengesRepository, ChallengesRepository>();
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
