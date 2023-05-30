using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenCommand : IRequest<CreateTokenResponse>, IMapTo<Token>
{
    public byte[] Content { get; set; }
    public DateTime ExpiresAt { get; set; }
}
