using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Devices.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Devices.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DatabaseConfiguration> setupOptions)
    {
        var options = new DatabaseConfiguration();
        setupOptions(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration options)
    {
        services
            .AddDbContext<DevicesDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Devices");
                        });
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(options.CommandTimeout);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryConfiguration.MaxRetryCount, TimeSpan.FromSeconds(options.RetryConfiguration.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Devices");
                        });
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }

                dbContextOptions.UseOpenIddict<
                    CustomOpenIddictEntityFrameworkCoreApplication,
                    CustomOpenIddictEntityFrameworkCoreAuthorization,
                    CustomOpenIddictEntityFrameworkCoreScope,
                    CustomOpenIddictEntityFrameworkCoreToken,
                    string>();
            });
        services.AddScoped<IDevicesDbContext, DevicesDbContext>();

        services.AddRepositories();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IIdentitiesRepository, IdentitiesRepository>();
        services.AddTransient<ITiersRepository, TiersRepository>();
        services.AddTransient<IChallengesRepository, ChallengesRepository>();
        services.AddTransient<IOAuthClientsRepository, OAuthClientsRepository>();
        services.AddTransient<IPnsRegistrationsRepository, PnsRegistrationsRepository>();
    }
}
