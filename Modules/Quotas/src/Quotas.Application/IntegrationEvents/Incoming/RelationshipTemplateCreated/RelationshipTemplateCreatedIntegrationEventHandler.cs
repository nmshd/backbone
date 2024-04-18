using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipTemplateCreated;

public class RelationshipTemplateCreatedIntegrationEventHandler : IIntegrationEventHandler<RelationshipTemplateCreatedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipTemplateCreatedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipTemplateCreatedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<MetricKey> { MetricKey.NumberOfRelationshipTemplates };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
