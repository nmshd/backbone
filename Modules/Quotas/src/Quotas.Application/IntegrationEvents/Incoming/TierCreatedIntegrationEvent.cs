using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
public class TierCreatedIntegrationEvent : IntegrationEvent
{
    public string Id { get; set; }

    public string Name { get; set; }
}
