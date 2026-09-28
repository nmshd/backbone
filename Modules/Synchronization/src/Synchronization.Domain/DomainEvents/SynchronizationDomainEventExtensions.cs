using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents;

public static class DatawalletModifiedDomainEventExtensions
{
    extension(DatawalletModifiedDomainEvent)
    {
        public static DatawalletModifiedDomainEvent Create(IdentityAddress identity, DeviceId modifiedByDevice) => new()
        {
            DomainEventId = $"{identity}/Datawallet/Modified/{Guid.NewGuid()}",
            Identity = identity,
            ModifiedByDevice = modifiedByDevice
        };
    }
}

public static class ExternalEventCreatedDomainEventExtensions
{
    extension(ExternalEventCreatedDomainEvent)
    {
        public static ExternalEventCreatedDomainEvent Create(ExternalEvent externalEvent) => new()
        {
            DomainEventId = $"{externalEvent.Id}/Created",
            EventId = externalEvent.Id,
            Owner = externalEvent.Owner,
            IsDeliveryBlocked = externalEvent.IsDeliveryBlocked
        };
    }
}
