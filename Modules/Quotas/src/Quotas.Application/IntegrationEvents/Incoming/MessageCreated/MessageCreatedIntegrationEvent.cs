using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.MessageCreated;
public class MessageCreatedIntegrationEvent : IntegrationEvent
{
    public string Id { get; private set; }
    public IEnumerable<string> Recipients { get; private set; }
}
