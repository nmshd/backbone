using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokenAllocationsOfIdentity;

public class AnonymizeTokenAllocationsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
