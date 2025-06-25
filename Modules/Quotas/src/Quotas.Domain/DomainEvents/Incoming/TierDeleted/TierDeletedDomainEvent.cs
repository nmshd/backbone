using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierDeleted;

public class TierDeletedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
}
