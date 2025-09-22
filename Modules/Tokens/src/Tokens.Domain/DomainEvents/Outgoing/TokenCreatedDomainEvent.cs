using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Domain.DomainEvents.Outgoing;

public class TokenCreatedDomainEvent : DomainEvent
{
    public TokenCreatedDomainEvent(Token newToken) : base($"{newToken.Id}/Created")
    {
        TokenId = newToken.Id;
        CreatedBy = newToken.CreatedBy?.Value;
    }

    public string TokenId { get; set; }
    public string? CreatedBy { get; set; }
}
