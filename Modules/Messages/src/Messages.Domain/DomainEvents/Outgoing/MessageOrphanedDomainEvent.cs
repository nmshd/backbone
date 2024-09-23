using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;

public class MessageOrphanedDomainEvent : DomainEvent
{
    /**
     * This constructor is used by the deserializer.
     */
    public MessageOrphanedDomainEvent()
    {
        MessageId = null!;
    }

    public MessageOrphanedDomainEvent(Message message) : base($"{message.Id}/MessageOrphaned")
    {
        MessageId = message.Id;
    }

    public string MessageId { get; set; }
}
