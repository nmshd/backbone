using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedIntegrationEventHandler : IIntegrationEventHandler<RelationshipStatusChangedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipStatusChangedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipStatusChangedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.Initiator, @event.Peer };
        var metrics = new List<MetricKey> { MetricKey.NumberOfRelationships };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
