using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Metrics;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration options)
    {
        services.AddDbContextForModule<QuotasDbContext>(options, "Quotas");

        services.AddTransient<IIdentitiesRepository, IdentitiesRepository>();
        services.AddTransient<IMessagesRepository, MessagesRepository>();
        services.AddTransient<IFilesRepository, FilesRepository>();
        services.AddTransient<IMetricsRepository, MetricsRepository>();
        services.AddTransient<ITiersRepository, TiersRepository>();
        services.AddTransient<IRelationshipsRepository, RelationshipsRepository>();
        services.AddTransient<IRelationshipTemplatesRepository, RelationshipTemplatesRepository>();
        services.AddTransient<ITokensRepository, TokensRepository>();
        services.AddTransient<MetricCalculatorFactory, ServiceProviderMetricCalculatorFactory>();
        services.AddTransient<IIdentityDeletionProcessesRepository, IdentityDeletionProcessesRepository>();
        services.AddTransient<IChallengesRepository, ChallengesRepository>();
        services.AddTransient<IDatawalletModificationsRepository, DatawalletModificationsRepository>();
        services.AddTransient<IDevicesRepository, DevicesRepository>();
    }
}
