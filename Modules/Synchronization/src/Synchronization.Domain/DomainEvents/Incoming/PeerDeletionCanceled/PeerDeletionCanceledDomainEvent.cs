using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCanceled;
public class PeerDeletionCanceledDomainEvent : DomainEvent
{
    public PeerDeletionCanceledDomainEvent(string peerOfIdentityWithDeletionCanceled, string relationshipId, string identityWithDeletionCanceled)
    {
        PeerOfIdentityWithDeletionCanceled = peerOfIdentityWithDeletionCanceled;
        RelationshipId = relationshipId;
        IdentityWithDeletionCanceled = identityWithDeletionCanceled;
    }

    public string PeerOfIdentityWithDeletionCanceled { get; }
    public string RelationshipId { get; }
    public string IdentityWithDeletionCanceled { get; }
}
