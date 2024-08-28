using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;

public class DeleteFilesOfIdentityCommand : IRequest
{
    public DeleteFilesOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
