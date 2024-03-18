using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

public class TierCreatedIntegrationEvent : IntegrationEvent
{
    public TierCreatedIntegrationEvent(Tier tier) : base($"{tier.Id}/Created")
    {
        Id = tier.Id;
        Name = tier.Name;
    }

    public string Id { get; }
    public string Name { get; }
}
