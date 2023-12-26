using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevicesOfIdentity;

public class DeleteDevicesOfIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
