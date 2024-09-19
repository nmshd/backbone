using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;

public class DeletePnsRegistrationsOfIdentityCommand : IRequest
{
    public DeletePnsRegistrationsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
