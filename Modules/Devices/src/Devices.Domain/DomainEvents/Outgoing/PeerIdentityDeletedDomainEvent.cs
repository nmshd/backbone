using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class PeerIdentityDeletedDomainEvent : DomainEvent
{
    public PeerIdentityDeletedDomainEvent(string relationshipId, IdentityAddress identityAddress) : base($"{relationshipId}/PeerIdentityDeleted/${identityAddress}")
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
}
