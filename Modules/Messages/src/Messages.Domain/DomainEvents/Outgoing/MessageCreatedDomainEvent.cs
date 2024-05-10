using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;

public class MessageCreatedDomainEvent : DomainEvent
{
    public MessageCreatedDomainEvent(Message message) : base($"{message.Id}/Created")
    {
        Id = message.Id.Value;
        Recipients = message.Recipients.Select(r => r.Address.ToString());
        CreatedBy = message.CreatedBy.Value;
    }

    public string Id { get; }
    public IEnumerable<string> Recipients { get; }
    public string CreatedBy { get; }
}
