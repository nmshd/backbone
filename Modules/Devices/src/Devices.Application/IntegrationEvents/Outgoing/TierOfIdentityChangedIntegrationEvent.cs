using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Devices.Domain.Aggregates.Tier;
using Backbone.Devices.Domain.Entities;

namespace Backbone.Devices.Application.IntegrationEvents.Outgoing;
public class TierOfIdentityChangedIntegrationEvent : IntegrationEvent
{
    public TierOfIdentityChangedIntegrationEvent(Identity identity, Tier oldTier, Tier newTier) : base($"{identity.Address}/TierOfIdentityChanged")
    {
        OldTier = oldTier.Id;
        NewTier = newTier.Id;
        IdentityAddress = identity.Address;
    }

    public string OldTier { get; set; }
    public string NewTier { get; set; }
    public string IdentityAddress { get; set; }
}
