using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierQuotaDefinitionDeleted;

public class TierQuotaDefinitionDeletedDomainEventHandler : IDomainEventHandler<TierQuotaDefinitionDeletedDomainEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IMetricStatusesService _metricStatusesService;
    private readonly ILogger<TierQuotaDefinitionDeletedDomainEventHandler> _logger;

    public TierQuotaDefinitionDeletedDomainEventHandler(IIdentitiesRepository identitiesRepository, IMetricStatusesService metricStatusesService,
        ILogger<TierQuotaDefinitionDeletedDomainEventHandler> logger)
    {
        _metricStatusesService = metricStatusesService;
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task Handle(TierQuotaDefinitionDeletedDomainEvent @event)
    {
        _logger.LogTrace("Handling '{eventName}' ... ", nameof(TierQuotaDefinitionDeletedDomainEvent));
        await RecalculateMetricStatuses(@event);
    }

    private async Task RecalculateMetricStatuses(TierQuotaDefinitionDeletedDomainEvent @event)
    {
        var identitiesWithTier = await _identitiesRepository.ListWithTier(TierId.Parse(@event.TierId), CancellationToken.None);

        await _metricStatusesService.RecalculateMetricStatuses(
            identitiesWithTier.Select(i => i.Address).ToList(),
            MetricKey.GetSupportedMetricKeys().ToList(),
            MetricUpdateType.OnlyExhausted,
            CancellationToken.None
        );
    }
}
