using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
public class DeleteIdentityCommand : IRequest
{
    public DeleteIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
