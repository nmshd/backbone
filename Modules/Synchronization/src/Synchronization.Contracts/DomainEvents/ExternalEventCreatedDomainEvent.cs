using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Contracts.DomainEvents;

public class ExternalEventCreatedDomainEvent : DomainEvent
{
    public required string EventId { get; set; }
    public required string Owner { get; set; }
    public required bool IsDeliveryBlocked { get; set; }
}
