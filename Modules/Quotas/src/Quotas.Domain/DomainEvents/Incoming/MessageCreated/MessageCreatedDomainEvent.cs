using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.MessageCreated;

public class MessageCreatedDomainEvent : DomainEvent
{
    public required string CreatedBy { get; set; }
}
