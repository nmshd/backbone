using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipChangeCreated;

public class RelationshipChangeCreatedDomainEvent : DomainEvent
{
    public required string ChangeId { get; set; }
    public required string RelationshipId { get; set; }
    public required string ChangeCreatedBy { get; set; }
    public required string ChangeRecipient { get; set; }
}
