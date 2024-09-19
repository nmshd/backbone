using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;

public class MessageOrphanedDomainEvent : DomainEvent
{
    public MessageOrphanedDomainEvent(Message message) : base($"{message.Id}/MessageOrphaned")
    {
        MessageId = message.Id;
        CreatedBy = message.CreatedBy;
    }

    public string MessageId { get; }
    public string CreatedBy { get; }
}
