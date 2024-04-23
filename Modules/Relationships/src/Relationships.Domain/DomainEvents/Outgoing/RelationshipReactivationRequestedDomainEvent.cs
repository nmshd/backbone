using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
public class RelationshipReactivationRequestedDomainEvent : DomainEvent
{
    public RelationshipReactivationRequestedDomainEvent(Relationship relationship, IdentityAddress requestingIdentity, IdentityAddress peer) :
        base($"{relationship.Id}/ReactivationRequested/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship.Id;
        RequestingIdentity = requestingIdentity.StringValue;
        Peer = peer.StringValue;
    }


    public string RelationshipId { get; }
    public string RequestingIdentity { get; set; }
    public string Peer { get; }
}
