using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

public class IdentityDeletionCancelledDomainEvent : DomainEvent
{
    public IdentityDeletionCancelledDomainEvent(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}
