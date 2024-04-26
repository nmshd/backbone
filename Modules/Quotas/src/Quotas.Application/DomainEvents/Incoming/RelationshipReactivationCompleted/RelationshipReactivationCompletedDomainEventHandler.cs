using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
public class RelationshipReactivationCompletedDomainEventHandler : IDomainEventHandler<RelationshipReactivationCompletedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipReactivationCompletedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipReactivationCompletedDomainEvent @event)
    {
        var identities = new List<string> { @event.Initiator, @event.Peer };
        var metrics = new List<MetricKey> { MetricKey.NumberOfRelationships };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
