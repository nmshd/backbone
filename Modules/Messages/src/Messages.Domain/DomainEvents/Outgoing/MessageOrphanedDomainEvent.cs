using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;

public class MessageOrphanedDomainEvent : DomainEvent
{
    public MessageOrphanedDomainEvent(Message message, IdentityAddress identityAddress) : base($"{message.Id}/Orphaned")
    {
        MessageId = message.Id;
        IdentityAddress = identityAddress;
    }

    public string MessageId { get; set; }
    public string IdentityAddress { get; set; }
}
