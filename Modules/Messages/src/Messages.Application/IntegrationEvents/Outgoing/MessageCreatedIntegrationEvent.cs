using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Messages.Domain.Entities;

namespace Messages.Application.IntegrationEvents.Outgoing;

public class MessageCreatedIntegrationEvent : IntegrationEvent
{
    public MessageCreatedIntegrationEvent(Message message) : base($"{message.Id}/Created")
    {
        Id = message.Id;
        Recipients = message.Recipients.Select(r => r.Address.ToString());
    }

    public string Id { get; }
    public IEnumerable<string> Recipients { get; }
}
