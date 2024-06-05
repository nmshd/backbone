using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Incoming.DatawalletModificationCreated;

public class DatawalletModifiedDomainEvent : DomainEvent
{
    public required IdentityAddress Identity { get; set; }
    public required DeviceId ModifiedByDevice { get; set; }
}
