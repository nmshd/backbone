using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.CreateToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateTokenResponse")]
public class Response
{
    public Response(Token token)
    {
        Id = token.Id;
        CreatedAt = token.CreatedAt;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
