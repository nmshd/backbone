using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;

public class RelationshipChangeCreatedIntegrationEventHandler : IIntegrationEventHandler<RelationshipChangeCreatedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipChangeCreatedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipChangeCreatedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.ChangeCreatedBy, @event.ChangeRecipient };
        var metrics = new List<string> { MetricKey.NumberOfRelationships.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}

