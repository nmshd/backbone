using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;

public class TierCreatedIntegrationEvent : IntegrationEvent
{
    public TierCreatedIntegrationEvent(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
}
