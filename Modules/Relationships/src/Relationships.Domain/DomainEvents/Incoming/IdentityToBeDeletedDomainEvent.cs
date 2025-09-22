using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; init; }
    public required DateTime GracePeriodEndsAt { get; init; }
}
