using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Quotas.Module.Features.Metrics.Shared;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Module.Features.DomainEvents.MessageCreated;

public class Handler : IDomainEventHandler<MessageCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public Handler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(MessageCreatedDomainEvent domainEvent)
    {
        var identities = new List<string> { domainEvent.CreatedBy };
        var metrics = new List<MetricKey> { MetricKey.NUMBER_OF_SENT_MESSAGES };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, MetricUpdateType.All, CancellationToken.None);
    }
}
