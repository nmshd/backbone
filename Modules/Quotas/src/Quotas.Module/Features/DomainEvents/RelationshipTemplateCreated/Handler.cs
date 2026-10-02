using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Module.Features.Metrics.Shared;
using Backbone.Modules.Relationships.Contracts.DomainEvents;

namespace Backbone.Modules.Quotas.Module.Features.DomainEvents.RelationshipTemplateCreated;

public class Handler : IDomainEventHandler<RelationshipTemplateCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public Handler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(RelationshipTemplateCreatedDomainEvent @event)
    {
        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<MetricKey> { MetricKey.NUMBER_OF_RELATIONSHIP_TEMPLATES };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, MetricUpdateType.All, CancellationToken.None);
    }
}
