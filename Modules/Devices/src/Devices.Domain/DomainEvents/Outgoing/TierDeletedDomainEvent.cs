using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class TierDeletedDomainEvent : DomainEvent
{
    public TierDeletedDomainEvent(Tier tier) : base($"{tier.Id.Value}/Deleted")
    {
        Id = tier.Id;
    }

    public string Id { get; }
}
