using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;

public class DeleteTokensOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
