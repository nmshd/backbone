using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class RelationshipStatusChangedDomainEvent : DomainEvent
{
    public RelationshipStatusChangedDomainEvent(Relationship relationship) : base(
        $"{relationship.Id}/StatusChanged/{relationship.AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedAt.ToUniversalString()}")
    {
        RelationshipId = relationship.Id;
        NewStatus = relationship.Status.ToString();
        Initiator = relationship.LastModifiedBy;
        Peer = relationship.GetPeerOf(relationship.LastModifiedBy);
        WasDueToIdentityDeletion = false;
    }

    public RelationshipStatusChangedDomainEvent(string relationshipId, string newStatus, string initiator, string peer, bool wasDueToIdentityDeletion)
    {
        RelationshipId = relationshipId;
        NewStatus = newStatus;
        Initiator = initiator;
        Peer = peer;
        WasDueToIdentityDeletion = wasDueToIdentityDeletion;
    }

    public string RelationshipId { get; set; }
    public string NewStatus { get; set; }
    public string Initiator { get; set; }
    public string Peer { get; set; }
    public bool WasDueToIdentityDeletion { get; set; }
}
