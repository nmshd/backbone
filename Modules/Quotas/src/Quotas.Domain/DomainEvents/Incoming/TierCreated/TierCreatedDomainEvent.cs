using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierCreated;

public class TierCreatedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
    public required string Name { get; set; }
}
