using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class TierCreatedDomainEvent : DomainEvent
{
    public TierCreatedDomainEvent(Tier tier) : base($"{tier.Id}/Created")
    {
        Id = tier.Id;
        Name = tier.Name;
    }

    public string Id { get; }
    public string Name { get; }
}
