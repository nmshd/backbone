using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

public class FeatureFlagsOfIdentityChangedDomainEvent : DomainEvent
{
    public IdentityAddress IdentityAddress { get; set; } = null!;
}
