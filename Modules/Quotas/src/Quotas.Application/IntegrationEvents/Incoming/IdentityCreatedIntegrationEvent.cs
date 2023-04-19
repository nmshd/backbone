using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
public class IdentityCreatedIntegrationEvent : IntegrationEvent
{
    public string Address { get; set; }

    public string TierId { get; set; }
}
