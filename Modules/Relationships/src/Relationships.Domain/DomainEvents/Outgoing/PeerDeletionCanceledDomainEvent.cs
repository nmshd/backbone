using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class PeerDeletionCanceledDomainEvent : DomainEvent
{
    public PeerDeletionCanceledDomainEvent(IdentityAddress identityAddress, string relationshipId, string peerIdentityAddress)
        : base($"{relationshipId}/PeerDeletionCanceled/{peerIdentityAddress}", randomizeId: true)
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
        PeerIdentityAddress = peerIdentityAddress;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
    public string PeerIdentityAddress { get; }
}
