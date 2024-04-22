using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class RelationshipStatusChangedDomainEvent : DomainEvent
{
    public RelationshipStatusChangedDomainEvent(Relationship relationship) : base($"{relationship.Id}/StatusChanged/{relationship.AuditLog.Last().CreatedAt.ToUniversalString()}")
    {
        RelationshipId = relationship.Id;
        Status = relationship.Status.ToString();
        Initiator = relationship.LastModifiedBy;
        Peer = relationship.LastModifiedBy == relationship.From ? relationship.To : relationship.From;
    }

    public string RelationshipId { get; set; }
    public string Status { get; set; }
    public string Initiator { get; set; }
    public string Peer { get; set; }
}
