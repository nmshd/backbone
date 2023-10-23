using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Tokens.Domain.Entities;

namespace Backbone.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenResponse : IMapTo<Token>
{
    public TokenId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
