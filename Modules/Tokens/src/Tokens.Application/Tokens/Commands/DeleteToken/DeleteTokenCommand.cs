using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteToken;

public class DeleteTokenCommand : IRequest
{
    public required string Id { get; set; }
}
