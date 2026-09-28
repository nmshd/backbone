using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Contracts.DomainEvents;

public class DatawalletModifiedDomainEvent : DomainEvent
{
    public required string Identity { get; set; }
    public required string ModifiedByDevice { get; set; }
}
