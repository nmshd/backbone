using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;

public class DeletePnsRegistrationsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
