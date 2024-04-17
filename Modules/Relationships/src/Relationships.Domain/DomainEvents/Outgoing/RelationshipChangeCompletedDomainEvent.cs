using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class RelationshipChangeCompletedDomainEvent : DomainEvent
{
    public RelationshipChangeCompletedDomainEvent(RelationshipChange change) : base($"{change.Id}/Completed")
    {
        ChangeId = change.Id;
        RelationshipId = change.RelationshipId;
        ChangeCreatedBy = change.Request.CreatedBy;
        ChangeRecipient = change.Request.CreatedBy == change.Relationship.From ? change.Relationship.To : change.Relationship.From;
        ChangeResult = MapStatusToResult(change.Status);
    }

    public string ChangeId { get; }
    public string RelationshipId { get; }
    public string ChangeCreatedBy { get; }
    public string ChangeRecipient { get; }
    public string ChangeResult { get; }

    private static string MapStatusToResult(RelationshipChangeStatus status)
    {
        return status switch
        {
            RelationshipChangeStatus.Accepted => "Accepted",
            RelationshipChangeStatus.Rejected => "Rejected",
            RelationshipChangeStatus.Revoked => "Revoked",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
