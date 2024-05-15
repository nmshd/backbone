using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;

public class ExternalEventCreatedDomainEvent : DomainEvent
{
    public ExternalEventCreatedDomainEvent(ExternalEvent externalEvent) : base($"{externalEvent.Id}/Created")
    {
        EventId = externalEvent.Id;
        Owner = externalEvent.Owner;
    }

    public string EventId { get; }
    public string Owner { get; }
}
