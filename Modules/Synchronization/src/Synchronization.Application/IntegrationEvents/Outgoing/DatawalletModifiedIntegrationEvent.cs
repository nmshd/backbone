using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Synchronization.Application.IntegrationEvents.Outgoing;

public class DatawalletModifiedIntegrationEvent : IntegrationEvent
{
    public DatawalletModifiedIntegrationEvent(IdentityAddress identity, DeviceId modifiedByDevice) : base(Guid.NewGuid().ToString())
    {
        Identity = identity;
        ModifiedByDevice = modifiedByDevice;
    }

    public IdentityAddress Identity { get; }
    public DeviceId ModifiedByDevice { get; }
}
