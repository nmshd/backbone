using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Module.Features.Metrics.Shared;
using Backbone.Modules.Relationships.Contracts.DomainEvents;

namespace Backbone.Modules.Quotas.Module.Features.DomainEvents.RelationshipStatusChanged;

public class Handler : IDomainEventHandler<RelationshipStatusChangedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public Handler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        var identities = new List<string> { @event.Initiator, @event.Peer };
        var metrics = new List<MetricKey> { MetricKey.NUMBER_OF_RELATIONSHIPS };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, MetricUpdateType.All, CancellationToken.None);
    }
}
