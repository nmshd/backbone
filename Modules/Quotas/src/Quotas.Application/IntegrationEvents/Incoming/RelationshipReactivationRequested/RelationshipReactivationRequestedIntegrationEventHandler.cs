using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipReactivationRequested;
public class RelationshipReactivationRequestedIntegrationEventHandler : IIntegrationEventHandler<RelationshipReactivationRequestedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipReactivationRequestedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipReactivationRequestedIntegrationEvent integrationEvent)
    {
        var identities = new List<string> { integrationEvent.CreatedBy };
        var metrics = new List<string> { MetricKey.NumberOfRelationships.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
