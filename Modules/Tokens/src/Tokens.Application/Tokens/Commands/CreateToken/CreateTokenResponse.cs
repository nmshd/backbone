using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenResponse : IMapTo<Token>
{
    public required TokenId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
