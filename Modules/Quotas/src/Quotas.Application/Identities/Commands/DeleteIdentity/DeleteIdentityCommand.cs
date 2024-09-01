using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;

public class DeleteIdentityCommand : IRequest
{
    public DeleteIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
