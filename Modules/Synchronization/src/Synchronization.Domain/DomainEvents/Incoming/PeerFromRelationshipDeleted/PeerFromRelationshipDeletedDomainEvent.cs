using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFromRelationshipDeleted;
public class PeerFromRelationshipDeletedDomainEvent : DomainEvent
{
    public PeerFromRelationshipDeletedDomainEvent(string identityAddress, string relationshipId, string peerAddress)
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
        PeerAddress = peerAddress;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
    public string PeerAddress { get; }
}
