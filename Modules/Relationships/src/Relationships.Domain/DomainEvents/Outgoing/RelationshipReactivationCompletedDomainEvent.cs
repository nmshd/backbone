using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class RelationshipReactivationCompletedDomainEvent : DomainEvent
{
    public RelationshipReactivationCompletedDomainEvent(Relationship relationship, IdentityAddress peer) :
        base($"{relationship.Id}/Reactivation/Completed/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship.Id;
        Peer = peer.Value;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
