using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteDeviceCommand")]
public class Command : IRequest
{
    public required string DeviceId { get; init; }
}
