using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class PeerToBeDeletedDomainEvent : DomainEvent
{
    public PeerToBeDeletedDomainEvent(string peerOfIdentityToBeDeleted, string relationshipId, string identityToBeDeleted)
        : base($"{relationshipId}/peerToBeDeleted/{identityToBeDeleted}", randomizeId: true)
    {
        PeerOfIdentityToBeDeleted = peerOfIdentityToBeDeleted;
        RelationshipId = relationshipId;
        IdentityToBeDeleted = identityToBeDeleted;
    }

    public string PeerOfIdentityToBeDeleted { get; }
    public string RelationshipId { get; }
    public string IdentityToBeDeleted { get; }
}
