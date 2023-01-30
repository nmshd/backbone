using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenResponse : IMapTo<Token>
{
    public TokenId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
