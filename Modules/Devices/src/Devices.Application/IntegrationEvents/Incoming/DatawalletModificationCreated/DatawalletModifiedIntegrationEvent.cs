using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated;

public class DatawalletModifiedIntegrationEvent : IntegrationEvent
{
    public IdentityAddress? Identity { get; set; }
    public DeviceId? ModifiedByDevice { get; set; }
}
