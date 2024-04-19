using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipChangeCreated;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipChangeCreated;

public class RelationshipChangeCreatedDomainEventHandler : IDomainEventHandler<RelationshipChangeCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipChangeCreatedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipChangeCreatedDomainEvent @event)
    {
        var identities = new List<string> { @event.ChangeCreatedBy, @event.ChangeRecipient };
        var metrics = new List<string> { MetricKey.NumberOfRelationships.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
