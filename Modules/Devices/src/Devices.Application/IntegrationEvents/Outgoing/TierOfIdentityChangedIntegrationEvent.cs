using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
public class TierOfIdentityChangedIntegrationEvent : IntegrationEvent
{
    public TierOfIdentityChangedIntegrationEvent(Identity identity, TierId oldTierIdId, TierId newTierIdId) : base($"{identity.Address}/TierOfIdentityChanged")
    {
        OldTierId = oldTierIdId;
        NewTierId = newTierIdId;
        IdentityAddress = identity.Address;
    }

    public string OldTierId { get; set; }
    public string NewTierId { get; set; }
    public string IdentityAddress { get; set; }
}
