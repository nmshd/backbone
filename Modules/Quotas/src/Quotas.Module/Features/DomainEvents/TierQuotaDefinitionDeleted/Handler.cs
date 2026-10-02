using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Contracts.DomainEvents;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Module.Features.Metrics.Shared;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Module.Features.DomainEvents.TierQuotaDefinitionDeleted;

public class Handler : IDomainEventHandler<TierQuotaDefinitionDeletedDomainEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IMetricStatusesService _metricStatusesService;
    private readonly ILogger<Handler> _logger;

    public Handler(IIdentitiesRepository identitiesRepository, IMetricStatusesService metricStatusesService,
        ILogger<Handler> logger)
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
