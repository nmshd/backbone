using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class PeerDeletedDomainEvent : DomainEvent
{
    public PeerDeletedDomainEvent(IdentityAddress identityAddress, string relationshipId, string peerIdentityAddress)
        : base($"{relationshipId}/PeerDeleted/{peerIdentityAddress}")
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
        PeerIdentityAddress = peerIdentityAddress;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
    public string PeerIdentityAddress { get; set; }
}
