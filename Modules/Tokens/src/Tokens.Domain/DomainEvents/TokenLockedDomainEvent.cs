using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Domain.DomainEvents;

public class TokenLockedDomainEvent : DomainEvent
{
    public TokenLockedDomainEvent(Token token) : base($"{token.Id}/Locked")
    {
        TokenId = token.Id;
        CreatedBy = token.CreatedBy;
    }

    public string TokenId { get; set; }
    public string CreatedBy { get; set; }
}
