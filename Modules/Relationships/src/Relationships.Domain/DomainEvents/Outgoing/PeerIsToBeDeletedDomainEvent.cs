using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class PeerIsToBeDeletedDomainEvent : DomainEvent
{
    public PeerIsToBeDeletedDomainEvent(string identityAddress, string relationshipId, string peerIdentityAddress)
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
        PeerIdentityAddress = peerIdentityAddress;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
    public string PeerIdentityAddress { get; }
}
