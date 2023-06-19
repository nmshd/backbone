using Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.MessageCreated;
public class MessageCreatedIntegrationEventHandler : IIntegrationEventHandler<MessageCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public MessageCreatedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(MessageCreatedIntegrationEvent integrationEvent)
    {
        var identities = new List<string> { integrationEvent.CreatedBy};
        var metrics = new List<string> { MetricKey.NumberOfSentMessages.Value };

        await _mediator.Send(new RecalculateMetricStatusesCommand(identities, metrics));
    }
}
