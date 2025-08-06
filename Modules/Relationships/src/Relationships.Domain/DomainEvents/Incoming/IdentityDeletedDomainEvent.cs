using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

public class IdentityDeletedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; init; }
}
