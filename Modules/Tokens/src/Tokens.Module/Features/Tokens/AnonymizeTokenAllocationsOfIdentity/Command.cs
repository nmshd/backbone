using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokenAllocationsOfIdentity;

public class AnonymizeTokenAllocationsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
