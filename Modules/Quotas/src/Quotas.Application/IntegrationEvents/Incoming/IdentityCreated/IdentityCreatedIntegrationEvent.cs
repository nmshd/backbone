using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;
public class IdentityCreatedIntegrationEvent : IntegrationEvent
{
    public string Address { get; }

    public string Tier { get; }
}
