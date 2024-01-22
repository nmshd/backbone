using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

public class DeleteDeviceCommand : IRequest
{
    public string? DeviceId { get; set; }
}
