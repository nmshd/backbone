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

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        services
            .AddDbContext<DevicesDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                        });
                        dbContextOptions.UseOpenIddict<
                            CustomOpenIddictEntityFrameworkCoreApplication,
                            CustomOpenIddictEntityFrameworkCoreAuthorization,
                            CustomOpenIddictEntityFrameworkCoreScope,
                            CustomOpenIddictEntityFrameworkCoreToken,
                            string>();
                        dbContextOptions.UseModel(Modules.Devices.Infrastructure.CompiledModels.SqlServer.DevicesDbContextModel.Instance);
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Devices"); //TODO: Remove this once the issue with package 'Npgsql.EntityFrameworkCore.PostgreSQL' is fixed https://github.com/npgsql/efcore.pg/issues/2878
                        });
                        dbContextOptions.UseOpenIddict<
                            CustomOpenIddictEntityFrameworkCoreApplication,
                            CustomOpenIddictEntityFrameworkCoreAuthorization,
                            CustomOpenIddictEntityFrameworkCoreScope,
                            CustomOpenIddictEntityFrameworkCoreToken,
                            string>();
                        //dbContextOptions.UseModel(Modules.Devices.Infrastructure.CompiledModels.Postgres.DevicesDbContextModel.Instance); TODO: Add this when issues with PostgreSQL compiled models are fixed https://github.com/npgsql/efcore.pg/issues/2972
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
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
