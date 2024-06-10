using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class PeerToBeDeletedDomainEvent : DomainEvent
{
    public PeerToBeDeletedDomainEvent(string identityAddress, string relationshipId, string peerIdentityAddress)
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
        PeerIdentityAddress = peerIdentityAddress;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
    public string PeerIdentityAddress { get; }
}
