using Backbone.Modules.Tokens.Contracts.DomainEvents;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Domain.DomainEvents;

public static class TokenCreatedDomainEventExtensions
{
    extension(TokenCreatedDomainEvent)
    {
        public static TokenCreatedDomainEvent Create(Token token) => new()
        {
            DomainEventId = $"{token.Id}/Created",
            TokenId = token.Id,
            CreatedBy = token.CreatedBy?.Value
        };
    }
}

public static class TokenLockedDomainEventExtensions
{
    extension(TokenLockedDomainEvent)
    {
        public static TokenLockedDomainEvent Create(Token token) => new()
        {
            DomainEventId = $"{token.Id}/Locked",
            TokenId = token.Id,
            CreatedBy = token.CreatedBy?.Value
        };
    }
}
