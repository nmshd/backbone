using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;

public class DatawalletModifiedDomainEvent : DomainEvent
{
    public required IdentityAddress Identity { get; set; }
    public required DeviceId ModifiedByDevice { get; set; }
}
