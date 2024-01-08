using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevicesOfIdentity;

public class DeleteDevicesOfIdentityCommand : IRequest
{
    public DeleteDevicesOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
