using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;

public class IdentityCreatedIntegrationEvent : IntegrationEvent
{
    public IdentityCreatedIntegrationEvent(string address, string tier)
    {
        Address = address;
        Tier = tier;
    }

    public string Address { get; private set; }

    public string Tier { get; private set; }
}
