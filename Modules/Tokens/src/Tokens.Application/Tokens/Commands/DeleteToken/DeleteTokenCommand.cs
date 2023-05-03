using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteToken;

public class DeleteTokenCommand : IRequest
{
    public TokenId Id { get; set; }
}
