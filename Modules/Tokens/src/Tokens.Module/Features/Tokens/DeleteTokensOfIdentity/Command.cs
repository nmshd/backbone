using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
