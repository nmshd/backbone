using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.PeerIdentityToBeDeleted;
public class PeerIdentityToBeDeletedDomainEvent : DomainEvent
{
    public PeerIdentityToBeDeletedDomainEvent(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; }
}
