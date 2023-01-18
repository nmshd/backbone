using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.IntegrationEvents.Outgoing;

public class ExternalEventCreatedIntegrationEvent : IntegrationEvent
{
    public ExternalEventCreatedIntegrationEvent(ExternalEvent externalEvent) : base($"{externalEvent.Id}/Created")
    {
        EventId = externalEvent.Id;
        Owner = externalEvent.Owner;
    }

    public string EventId { get; }
    public string Owner { get; }
}
