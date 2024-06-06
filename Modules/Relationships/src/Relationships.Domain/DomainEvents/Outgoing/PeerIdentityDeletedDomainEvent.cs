using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class PeerIdentityDeletedDomainEvent : DomainEvent
{
    public PeerIdentityDeletedDomainEvent(IdentityAddress identityAddress, string relationshipId) : base($"{identityAddress}/PeerIdentityDeleted/{relationshipId}")
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
}
