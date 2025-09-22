using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.DTOs;

public class TokenDTO
{
    public TokenDTO(Token token)
    {
        Id = token.Id;
        CreatedBy = token.CreatedBy?.Value;
        CreatedByDevice = token.CreatedByDevice?.Value;
        CreatedAt = token.CreatedAt;
        ExpiresAt = token.ExpiresAt;
        Content = token.Details.Content;
        ForIdentity = token.ForIdentity?.Value;
        IsPasswordProtected = token.Password is { Length: > 0 };
    }

    public string Id { get; }

    public string? CreatedBy { get; }
    public string? CreatedByDevice { get; }

    public string? ForIdentity { get; }

    public DateTime CreatedAt { get; }
    public DateTime ExpiresAt { get; }

    public byte[]? Content { get; }

    public bool IsPasswordProtected { get; }
}
