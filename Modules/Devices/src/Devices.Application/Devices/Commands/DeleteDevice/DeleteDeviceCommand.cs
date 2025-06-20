using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

public class DeleteDeviceCommand : IRequest
{
    public required string DeviceId { get; init; }
}
