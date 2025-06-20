using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class UpdateDeviceRegistrationCommand : IRequest<UpdateDeviceRegistrationResponse>
{
    public required string Platform { get; init; }
    public required string Handle { get; init; }
    public required string AppId { get; init; }
    public string? Environment { get; init; }
}
