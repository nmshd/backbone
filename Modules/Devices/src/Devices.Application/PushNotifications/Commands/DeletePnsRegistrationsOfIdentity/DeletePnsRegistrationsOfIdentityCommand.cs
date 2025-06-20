using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;

public class DeletePnsRegistrationsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
