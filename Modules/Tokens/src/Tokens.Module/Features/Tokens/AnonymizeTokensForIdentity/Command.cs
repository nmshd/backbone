using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokensForIdentity;

public class AnonymizeTokensForIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
