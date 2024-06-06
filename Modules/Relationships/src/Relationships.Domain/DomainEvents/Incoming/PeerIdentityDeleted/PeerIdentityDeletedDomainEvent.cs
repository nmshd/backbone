using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.PeerIdentityDeleted;
public class PeerIdentityDeletedDomainEvent : DomainEvent
{
    public PeerIdentityDeletedDomainEvent(string relationshipId, IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
}
