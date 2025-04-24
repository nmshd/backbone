using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.MessageCreated;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.MessageCreated;

public class MessageCreatedDomainEventHandler : IDomainEventHandler<MessageCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public MessageCreatedDomainEventHandler(IMetricStatusesService metricStatusesService)
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
