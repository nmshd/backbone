using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierQuotaDefinitionDeleted;
public class TierQuotaDefinitionDeletedIntegrationEventHandler : IIntegrationEventHandler<TierQuotaDefinitionDeletedIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IMetricStatusesService _metricStatusesService;
    private readonly ILogger<TierQuotaDefinitionDeletedIntegrationEventHandler> _logger;

    public TierQuotaDefinitionDeletedIntegrationEventHandler(IIdentitiesRepository identitiesRepository, IMetricStatusesService metricStatusesService, ILogger<TierQuotaDefinitionDeletedIntegrationEventHandler> logger)
    {
        _metricStatusesService = metricStatusesService;
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task Handle(TierQuotaDefinitionDeletedIntegrationEvent @event)
    {
        _logger.LogTrace("Handling '{eventName}' ... ", nameof(TierQuotaDefinitionDeletedIntegrationEvent));
        await RecalculateMetricStatuses(@event);
    }

    private async Task RecalculateMetricStatuses(TierQuotaDefinitionDeletedIntegrationEvent @event)
    {
        var identitiesWithTier = await _identitiesRepository.FindWithTier(new TierId(@event.TierId), CancellationToken.None);

        await _metricStatusesService.RecalculateMetricStatuses(
            identitiesWithTier.Select(i => i.Address).ToList(),
            MetricKey.GetSupportedMetricKeys().ToList(),
            CancellationToken.None
        );
    }
}
