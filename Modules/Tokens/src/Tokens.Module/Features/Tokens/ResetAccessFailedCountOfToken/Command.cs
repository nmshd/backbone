using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ResetAccessFailedCountOfToken;

public class Command : IRequest
{
    public required string TokenId { get; init; }
}
