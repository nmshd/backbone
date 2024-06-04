using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class TierOfIdentityChangedDomainEvent : DomainEvent
{
    public TierOfIdentityChangedDomainEvent(Identity identity, TierId oldTierIdId, TierId newTierId) : base($"{identity.Address}/TierOfIdentityChanged", randomizeId: true)
    {
        OldTierId = oldTierIdId;
        NewTierId = newTierId;
        IdentityAddress = identity.Address;
    }

    public string OldTierId { get; set; }
    public string NewTierId { get; set; }
    public string IdentityAddress { get; set; }
}
