using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenResponse
{
    public CreateTokenResponse(Token token)
    {
        Id = token.Id;
        CreatedAt = token.CreatedAt;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
