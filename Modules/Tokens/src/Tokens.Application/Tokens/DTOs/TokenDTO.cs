using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.DTOs;

public class TokenDTO
{
    public TokenDTO(Token token)
    {
        Id = token.Id;
        CreatedBy = token.CreatedBy;
        CreatedByDevice = token.CreatedByDevice;
        CreatedAt = token.CreatedAt;
        ExpiresAt = token.ExpiresAt;
        Content = token.Content;
        ForIdentity = token.ForIdentity?.Value;
        IsPasswordProtected = token.Password is { Length: > 0 };
    }

    public string Id { get; set; }

    public string CreatedBy { get; set; }
    public string CreatedByDevice { get; set; }

    public string? ForIdentity { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public byte[] Content { get; set; }

    public bool IsPasswordProtected { get; }
}
