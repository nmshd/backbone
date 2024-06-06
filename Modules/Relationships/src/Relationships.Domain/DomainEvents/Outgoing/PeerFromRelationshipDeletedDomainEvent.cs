using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class PeerFromRelationshipDeletedDomainEvent : DomainEvent
{
    public PeerFromRelationshipDeletedDomainEvent(IdentityAddress identityAddress, string relationshipId, string peerAddress) 
        : base($"{relationshipId}/PeerFromRelationshipDeleted/{peerAddress}")
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
        PeerAddress = peerAddress;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
    public string PeerAddress { get; set; }
}
