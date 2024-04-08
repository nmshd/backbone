using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;
public class RelationshipChangeCompletedIntegrationEventHandler : IIntegrationEventHandler<RelationshipChangeCompletedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipChangeCompletedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipChangeCompletedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.ChangeCreatedBy, @event.ChangeRecipient };
        var metrics = new List<string> { MetricKey.NumberOfRelationships.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
