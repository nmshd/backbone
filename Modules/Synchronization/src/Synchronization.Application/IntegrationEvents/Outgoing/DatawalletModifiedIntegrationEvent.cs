using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;

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
