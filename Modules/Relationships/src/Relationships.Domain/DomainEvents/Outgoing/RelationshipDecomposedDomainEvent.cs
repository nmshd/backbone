using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class RelationshipDecomposedDomainEvent : DomainEvent
{
    public RelationshipDecomposedDomainEvent(Relationship relationship, IdentityAddress peer) : base($"{relationship.Id}/Decompose/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship.Id;
        Peer = peer.StringValue;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
