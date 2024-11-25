using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UseBackupDevice;

public class UseBackupDeviceCommand : IRequest
{
    public required string DeviceId { get; set; }
}
