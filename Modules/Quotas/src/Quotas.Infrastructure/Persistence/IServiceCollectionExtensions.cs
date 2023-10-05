using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Metrics;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Quotas.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Quotas.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        switch (options.Provider)
        {
            case SQLSERVER:
                services
                    .AddSaveChangesTimeInterceptor()
                    .AddDbContext<QuotasDbContext>((provider, dbContextOptions) =>
                        {
                            dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(20);
                                sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            }).AddInterceptors(provider.GetRequiredService<SaveChangesTimeInterceptor>());
                        });
                break;
            case POSTGRES:
                services
                    .AddSaveChangesTimeInterceptor()
                    .AddDbContext<QuotasDbContext>((provider, dbContextOptions) =>
                        {
                            dbContextOptions.UseNpgsql(options.DbConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(20);
                                sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            }).AddInterceptors(provider.GetRequiredService<SaveChangesTimeInterceptor>());
                        });
                break;
            default:
                throw new Exception($"Unsupported database provider: {options.Provider}");

        }
        services.AddTransient<IIdentitiesRepository, IdentitiesRepository>();
        services.AddTransient<IMessagesRepository, MessagesRepository>();
        services.AddTransient<IFilesRepository, FilesRepository>();
        services.AddTransient<IMetricsRepository, MetricsRepository>();
        services.AddTransient<ITiersRepository, TiersRepository>();
        services.AddTransient<IRelationshipsRepository, RelationshipsRepository>();
        services.AddTransient<IRelationshipTemplatesRepository, RelationshipTemplatesRepository>();
        services.AddTransient<ITokensRepository, TokensRepository>();
        services.AddTransient<MetricCalculatorFactory, ServiceProviderMetricCalculatorFactory>();
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
}
