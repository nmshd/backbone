using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Domain.DomainEvents.Outgoing;

public class TokenLockedDomainEvent : DomainEvent
{
    public TokenLockedDomainEvent(Token token) : base($"{token.Id}/Locked")
    {
        TokenId = token.Id;
        CreatedBy = token.CreatedBy?.Value;
    }

    public string TokenId { get; set; }
    public string? CreatedBy { get; set; }
}
