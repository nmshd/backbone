using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Tokens.Domain.Entities;

namespace Backbone.Tokens.Application.IntegrationEvents;
public class TokenCreatedIntegrationEvent : IntegrationEvent
{
    public TokenCreatedIntegrationEvent(Token newToken) : base($"{newToken.Id}/Created")
    {
        TokenId = newToken.Id;
        CreatedBy = newToken.CreatedBy;
    }

    public string TokenId { get; set; }
    public string CreatedBy { get; set; }
}
