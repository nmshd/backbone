using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class UpdateDeviceRegistrationCommand : IRequest<UpdateDeviceRegistrationResponse>
{
    public required string Platform { get; set; }
    public required string Handle { get; set; }
    public required string AppId { get; set; }
    public string? Environment { get; set; }
}
