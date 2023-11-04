using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class UpdateDeviceRegistrationCommand : IRequest<UpdateDeviceRegistrationResponse>
{
    public string Platform { get; set; }
    public string Handle { get; set; }
    public string AppId { get; set; }
    public string? Environment { get; set; }
}
