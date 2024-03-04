using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierDeleted;

public class TierDeletedIntegrationEvent : IntegrationEvent
{
    public TierDeletedIntegrationEvent(string id)
    {
        Id = id;
    }

    public string Id { get; private set; }
}
