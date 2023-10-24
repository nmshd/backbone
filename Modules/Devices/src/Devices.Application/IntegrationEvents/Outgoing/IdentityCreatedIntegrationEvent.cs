using Backbone.Modules.Devices.Domain.Entities.Identities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
public class IdentityCreatedIntegrationEvent : IntegrationEvent
{
    public IdentityCreatedIntegrationEvent(Identity identity) : base($"{identity.Address}/Created")
    {
        Address = identity.Address;
        Tier = identity.TierId;
    }

    public string Address { get; }
    public string Tier { get; }
}
