using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
public class TierOfIdentityChangedIntegrationEvent : IntegrationEvent
{
    public TierOfIdentityChangedIntegrationEvent(Identity identity, TierId oldTierId, TierId newTierId) : base($"{identity.Address}/TierOfIdentityChanged")
    {
        OldTier = oldTierId;
        NewTier = newTierId;
        IdentityAddress = identity.Address;
    }

    public string OldTier { get; set; }
    public string NewTier { get; set; }
    public string IdentityAddress { get; set; }
}
