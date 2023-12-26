using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
public class DeleteIdentityCommand(string identityAddress) : IRequest
{
    public string IdentityAddress { get; set; } = identityAddress;
}
