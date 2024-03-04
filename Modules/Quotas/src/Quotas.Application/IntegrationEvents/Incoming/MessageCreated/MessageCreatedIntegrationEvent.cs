using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.MessageCreated;
public class MessageCreatedIntegrationEvent : IntegrationEvent
{
    public required string Id { get; set; }
    public required IEnumerable<string> Recipients { get; set; }
    public required string CreatedBy { get; set; }
}
