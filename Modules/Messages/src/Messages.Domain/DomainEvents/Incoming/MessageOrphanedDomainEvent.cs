using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Incoming;

public class MessageOrphanedDomainEvent : DomainEvent
{
    public required string MessageId { get; set; }
    public required string IdentityAddress { get; set; }
}
