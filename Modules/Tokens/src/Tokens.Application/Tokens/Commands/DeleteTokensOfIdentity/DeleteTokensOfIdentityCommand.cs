using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;

public class DeleteTokensOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
