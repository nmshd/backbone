using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;

public class DeleteDeviceCommand : IRequest
{
    public required string DeviceId { get; init; }
}
