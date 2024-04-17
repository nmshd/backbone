using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipTemplateCreated;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipTemplateCreated;

public class RelationshipTemplateCreatedDomainEventHandler : IDomainEventHandler<RelationshipTemplateCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public RelationshipTemplateCreatedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipTemplateCreatedDomainEvent @event)
    {
        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<string> { MetricKey.NumberOfRelationshipTemplates.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
