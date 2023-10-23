using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Devices.Domain.Aggregates.Tier;

namespace Backbone.Devices.Application.IntegrationEvents.Outgoing;

public class TierDeletedIntegrationEvent : IntegrationEvent
{
    public TierDeletedIntegrationEvent(Tier tier) : base($"{tier.Id.Value}/Deleted")
    {
        Id = tier.Id;
    }

    public string Id { get; }
}
