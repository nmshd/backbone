using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;

public class Command : IRequest
{
    public required string DeviceId { get; init; }
}
