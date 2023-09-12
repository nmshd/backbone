using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipTemplateCreated;
internal class RelationshipTemplateCreatedIntegrationEventHandler : IIntegrationEventHandler<RelationshipTemplateCreatedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipTemplateCreatedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipTemplateCreatedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<string> { MetricKey.NumberOfRelationshipTemplates.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
