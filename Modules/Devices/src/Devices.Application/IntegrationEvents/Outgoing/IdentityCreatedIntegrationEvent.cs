using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Devices.Domain.Entities.Identities;

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
