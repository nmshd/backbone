using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TokenCreated;
public class TokenCreatedIntegrationEvent : IntegrationEvent
{
    public string CreatedBy { get; set; }
}
