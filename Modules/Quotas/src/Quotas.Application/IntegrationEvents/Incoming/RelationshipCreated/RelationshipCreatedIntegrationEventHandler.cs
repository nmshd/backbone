using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipCreated;

public class RelationshipCreatedIntegrationEventHandler : IIntegrationEventHandler<RelationshipCreatedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipCreatedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipCreatedIntegrationEvent @event)
    {
        var affectedIdentities = new List<string> { @event.From };

        await _metricStatusesService.RecalculateMetricStatuses(affectedIdentities, [MetricKey.NumberOfRelationships], CancellationToken.None);
    }
}
