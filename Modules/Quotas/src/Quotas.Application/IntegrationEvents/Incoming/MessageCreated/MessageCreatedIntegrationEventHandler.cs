using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.MessageCreated;
public class MessageCreatedIntegrationEventHandler : IIntegrationEventHandler<MessageCreatedIntegrationEvent>
{
    private readonly MetricStatusesService _metricStatusesService;

    public MessageCreatedIntegrationEventHandler(MetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(MessageCreatedIntegrationEvent integrationEvent)
    {
        var identities = new List<string> { integrationEvent.CreatedBy };
        var metrics = new List<string> { MetricKey.NumberOfSentMessages.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
