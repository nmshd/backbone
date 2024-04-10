using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
public class DeleteIdentityCommand : IRequest
{
    public DeleteIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
