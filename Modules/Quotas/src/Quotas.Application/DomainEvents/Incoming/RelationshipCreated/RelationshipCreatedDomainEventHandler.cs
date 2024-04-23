using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipCreated;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipCreated;

public class RelationshipCreatedDomainEventHandler : IDomainEventHandler<RelationshipCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipCreatedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipCreatedDomainEvent @event)
    {
        var affectedIdentities = new List<string> { @event.From };

        await _metricStatusesService.RecalculateMetricStatuses(affectedIdentities, [MetricKey.NumberOfRelationships], CancellationToken.None);
    }
}
