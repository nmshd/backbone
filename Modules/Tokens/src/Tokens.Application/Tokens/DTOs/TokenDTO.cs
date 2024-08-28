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
    }

    public string Id { get; set; }

    public string CreatedBy { get; set; }
    public string CreatedByDevice { get; set; }

    public IdentityAddress? ForIdentity { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public byte[] Content { get; set; }
}
