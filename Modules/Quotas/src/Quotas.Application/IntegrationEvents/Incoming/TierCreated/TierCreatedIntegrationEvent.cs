using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
public class TierCreatedIntegrationEvent : IntegrationEvent
{
    public string Id { get; }

    public string Name { get; }
}
