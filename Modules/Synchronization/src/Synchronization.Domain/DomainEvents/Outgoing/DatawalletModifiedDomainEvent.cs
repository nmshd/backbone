using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;

public class DatawalletModifiedDomainEvent : DomainEvent
{
    public DatawalletModifiedDomainEvent(IdentityAddress identity, DeviceId modifiedByDevice) : base(Guid.NewGuid().ToString())
    {
        Identity = identity;
        ModifiedByDevice = modifiedByDevice;
    }

    public IdentityAddress Identity { get; }
    public DeviceId ModifiedByDevice { get; }
}
