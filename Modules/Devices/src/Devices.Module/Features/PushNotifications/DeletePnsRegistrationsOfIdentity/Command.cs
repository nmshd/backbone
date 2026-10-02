using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
