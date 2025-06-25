using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.ResetAccessFailedCountOfToken;

public class ResetAccessFailedCountOfTokenCommand : IRequest
{
    public required string TokenId { get; init; }
}
