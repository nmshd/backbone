using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class FeatureFlagsOfIdentityChangedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}
