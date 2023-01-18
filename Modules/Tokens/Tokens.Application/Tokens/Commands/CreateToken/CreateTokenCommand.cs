using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using MediatR;
using Tokens.Domain.Entities;

namespace Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenCommand : IRequest<CreateTokenResponse>, IMapTo<Token>
{
    public byte[] Content { get; set; }
    public DateTime ExpiresAt { get; set; }
}
