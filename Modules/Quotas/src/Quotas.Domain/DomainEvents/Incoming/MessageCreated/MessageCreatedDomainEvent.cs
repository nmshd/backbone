using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.MessageCreated;

public class MessageCreatedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
    public required IEnumerable<string> Recipients { get; set; }
    public required string CreatedBy { get; set; }
}
