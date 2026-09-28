using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class TierOfIdentityChangedDomainEvent : DomainEvent
{
    public required string OldTierId { get; set; }
    public required string NewTierId { get; set; }
    public required string IdentityAddress { get; set; }
}
