using MediatR;
using Tokens.Domain.Entities;

namespace Tokens.Application.Tokens.Commands.DeleteToken;

public class DeleteTokenCommand : IRequest<Unit>
{
    public TokenId Id { get; set; }
}
