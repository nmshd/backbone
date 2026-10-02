using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokensForIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
