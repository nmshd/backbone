using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

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
