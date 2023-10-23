using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Quotas.Application.IntegrationEvents.Incoming.TokenCreated;
public class TokenCreatedIntegrationEvent : IntegrationEvent
{
    public string CreatedBy { get; set; }
}
