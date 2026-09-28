using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class IdentityCreatedDomainEvent : DomainEvent
{
    public required string Address { get; set; }
    public required string Tier { get; set; }
}
