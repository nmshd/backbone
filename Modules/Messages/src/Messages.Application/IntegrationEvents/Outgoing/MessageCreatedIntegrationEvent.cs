using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Messages.Domain.Entities;

namespace Backbone.Messages.Application.IntegrationEvents.Outgoing;

public class MessageCreatedIntegrationEvent : IntegrationEvent
{
    public MessageCreatedIntegrationEvent(Message message) : base($"{message.Id}/Created")
    {
        Id = message.Id;
        Recipients = message.Recipients.Select(r => r.Address.ToString());
        CreatedBy = message.CreatedBy.StringValue;
    }

    public string Id { get; }
    public IEnumerable<string> Recipients { get; }
    public string CreatedBy { get; }
}
