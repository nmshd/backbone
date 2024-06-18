using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Incoming.DatawalletModificationCreated;

public class DatawalletModifiedDomainEvent : DomainEvent
{
    public required string Identity { get; set; }
    public required string ModifiedByDevice { get; set; }
}
