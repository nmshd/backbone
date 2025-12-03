using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenResponse
{
    public CreateTokenResponse(Token token)
    {
        Id = token.Id;
        CreatedAt = token.CreatedAt;
    }

    [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
    public string Id { get; set; }

    [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
    public DateTime CreatedAt { get; set; }
}
