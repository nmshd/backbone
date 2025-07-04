using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierDeleted;

public class TierDeletedDomainEvent : DomainEvent
{
    public TierDeletedDomainEvent(string id)
    {
        Id = id;
    }

    public string Id { get; private set; }
}
