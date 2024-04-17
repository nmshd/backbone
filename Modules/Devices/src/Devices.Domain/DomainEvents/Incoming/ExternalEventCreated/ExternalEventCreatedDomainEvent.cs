using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedDomainEvent : DomainEvent
{
    public required string Owner { get; set; }
}
