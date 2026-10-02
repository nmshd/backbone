using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokenAllocationsOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
