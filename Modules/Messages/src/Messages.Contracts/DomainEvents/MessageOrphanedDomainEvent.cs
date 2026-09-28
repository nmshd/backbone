using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Messages.Contracts.DomainEvents;

public class MessageOrphanedDomainEvent : DomainEvent
{
    public required string MessageId { get; set; }
}
