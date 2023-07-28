using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TokenCreated;
public class TokenCreatedIntegrationEvent : IntegrationEvent
{
    public string TokenId { get; set; }
    public string CreatedBy { get; set; }
}
