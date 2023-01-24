using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Tokens.Domain.Entities;

namespace Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenResponse : IMapTo<Token>
{
    public TokenId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
