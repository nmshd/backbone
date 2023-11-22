using MediatR;

namespace Backbone.Modules.Synchronization.Application.Identities.Commands.DeleteIdentity;
public class DeleteIdentityCommand : IRequest
{
    public string IdentityAddress { get; private set; }

    public DeleteIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }
}
