using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageOrphaned;

public class MessageOrphanedDomainEvent : DomainEvent
{
    public MessageOrphanedDomainEvent(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public required string IdentityAddress { get; set; }
}
