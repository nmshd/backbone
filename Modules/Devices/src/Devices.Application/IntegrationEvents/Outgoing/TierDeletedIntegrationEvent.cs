using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

public class TierDeletedIntegrationEvent : IntegrationEvent
{
    public TierDeletedIntegrationEvent(Tier tier) : base($"{tier.Id.Value}/Deleted")
    {
        Id = tier.Id;
    }

    public string Id { get; }
}
