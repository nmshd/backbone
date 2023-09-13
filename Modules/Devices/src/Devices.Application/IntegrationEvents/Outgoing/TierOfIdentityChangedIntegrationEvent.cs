using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
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
