using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class TierDeletedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
}
