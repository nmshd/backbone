using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipReactivationRequested;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipReactivationRequested;
public class RelationshipReactivationRequestedDomainEventHandler : IDomainEventHandler<RelationshipReactivationRequestedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipReactivationRequestedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipReactivationRequestedDomainEvent integrationEvent)
    {
        await _metricStatusesService.RecalculateMetricStatuses([integrationEvent.RequestingIdentity], [MetricKey.NumberOfRelationships], CancellationToken.None);
    }
}
