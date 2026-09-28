using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class TierCreatedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
    public required string Name { get; set; }
}
