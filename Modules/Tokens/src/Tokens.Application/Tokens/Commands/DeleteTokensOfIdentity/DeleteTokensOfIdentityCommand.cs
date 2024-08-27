using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;

public class DeleteTokensOfIdentityCommand : IRequest
{
    public DeleteTokensOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
