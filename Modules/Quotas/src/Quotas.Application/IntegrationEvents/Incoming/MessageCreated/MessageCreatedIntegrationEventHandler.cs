using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.MessageCreated;
public class MessageCreatedIntegrationEventHandler : IIntegrationEventHandler<MessageCreatedIntegrationEvent>
{
    Task IIntegrationEventHandler<MessageCreatedIntegrationEvent>.Handle(MessageCreatedIntegrationEvent integrationEvent)
    {
        Console.WriteLine(integrationEvent.ToString());
        return Task.CompletedTask;
    }
}
