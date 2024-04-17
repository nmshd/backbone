using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipTemplateCreated;

public class RelationshipTemplateCreatedDomainEvent : DomainEvent
{
    public required string CreatedBy { get; set; }
}
