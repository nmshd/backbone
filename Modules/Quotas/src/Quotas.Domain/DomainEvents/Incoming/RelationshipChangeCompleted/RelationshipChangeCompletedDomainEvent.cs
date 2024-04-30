using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipChangeCompleted;

public class RelationshipChangeCompletedDomainEvent : DomainEvent
{
    public required string ChangeId { get; set; }
    public required string RelationshipId { get; set; }
    public required string ChangeCreatedBy { get; set; }
    public required string ChangeRecipient { get; set; }
    public required string ChangeResult { get; set; }
}
