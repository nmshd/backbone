using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteIdentity;

public class DeleteIdentityCommand : IRequest
{
    public string IdentityAddress { get; private set; }

    public DeleteIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }
}
