using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityCreatedDomainEvent : DomainEvent
{
    public IdentityCreatedDomainEvent(Identity identity) : base($"{identity.Address}/Created")
    {
        Address = identity.Address;
        Tier = identity.TierId;
    }

    public string Address { get; }
    public string Tier { get; }
}
