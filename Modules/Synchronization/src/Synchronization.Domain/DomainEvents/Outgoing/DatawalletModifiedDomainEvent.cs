using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;

public class DatawalletModifiedDomainEvent : DomainEvent
{
    public DatawalletModifiedDomainEvent(IdentityAddress identity, DeviceId modifiedByDevice) : base($"{identity}/Datawallet/Modified/{Guid.NewGuid()}")
    {
        Identity = identity;
        ModifiedByDevice = modifiedByDevice;
    }

    public string Identity { get; }
    public string ModifiedByDevice { get; }
}
