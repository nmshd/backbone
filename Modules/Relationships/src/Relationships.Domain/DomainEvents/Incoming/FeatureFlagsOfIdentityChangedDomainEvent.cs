using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

public class FeatureFlagsOfIdentityChangedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; init; }
}
