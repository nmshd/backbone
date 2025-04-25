using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipStatusChanged;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedDomainEventHandler : IDomainEventHandler<RelationshipStatusChangedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipStatusChangedDomainEventHandler(IMetricStatusesService metricStatusesService)
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
