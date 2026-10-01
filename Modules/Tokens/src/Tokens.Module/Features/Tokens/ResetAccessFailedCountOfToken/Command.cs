using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ResetAccessFailedCountOfToken;

public class ResetAccessFailedCountOfTokenCommand : IRequest
{
    public required string TokenId { get; init; }
}
