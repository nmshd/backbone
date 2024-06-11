using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCanceled;
public class PeerDeletionCanceledDomainEvent : DomainEvent
{
    public PeerDeletionCanceledDomainEvent(IdentityAddress identityAddress, string relationshipId, string peerIdentityAddress)
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
        PeerIdentityAddress = peerIdentityAddress;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
    public string PeerIdentityAddress { get; }
}
