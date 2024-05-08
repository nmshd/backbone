using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class RelationshipChangeCreatedDomainEvent : DomainEvent
{
    public RelationshipChangeCreatedDomainEvent(RelationshipChange change) : base($"{change.Id}/Created")
    {
        ChangeId = change.Id;
        RelationshipId = change.RelationshipId;
        ChangeCreatedBy = change.Request.CreatedBy;
        ChangeRecipient = change.Request.CreatedBy == change.Relationship.From ? change.Relationship.To : change.Relationship.From;
    }

    public string ChangeId { get; }
    public string RelationshipId { get; }
    public string ChangeCreatedBy { get; }
    public string ChangeRecipient { get; }
}
