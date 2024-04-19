using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipChangeCompleted;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipChangeCompleted;

public class RelationshipChangeCompletedDomainEventHandler : IDomainEventHandler<RelationshipChangeCompletedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipChangeCompletedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipChangeCompletedDomainEvent @event)
    {
        var identities = new List<string> { @event.ChangeCreatedBy, @event.ChangeRecipient };
        var metrics = new List<string> { MetricKey.NumberOfRelationships.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
