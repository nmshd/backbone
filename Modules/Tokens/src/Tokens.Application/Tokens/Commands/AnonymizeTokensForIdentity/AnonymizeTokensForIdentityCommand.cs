using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokensForIdentity;

public class AnonymizeTokensForIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
