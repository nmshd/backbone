using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipReactivationRequestCreated;
public class RelationshipReactivationRequestIntegrationEventHandler : IIntegrationEventHandler<RelationshipReactivationRequestIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipReactivationRequestIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipReactivationRequestIntegrationEvent integrationEvent)
    {
        var identities = new List<string> { integrationEvent.CreatedBy };
        var metrics = new List<MetricKey> { MetricKey.NumberOfRelationships };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
