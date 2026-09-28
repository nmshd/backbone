using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Contracts.DomainEvents;

public class DatawalletModifiedDomainEvent : DomainEvent
{
    public required string Identity { get; set; }
    public required string ModifiedByDevice { get; set; }
}

public class ExternalEventCreatedDomainEvent : DomainEvent
{
    public required string EventId { get; set; }
    public required string Owner { get; set; }
    public required bool IsDeliveryBlocked { get; set; }
}
