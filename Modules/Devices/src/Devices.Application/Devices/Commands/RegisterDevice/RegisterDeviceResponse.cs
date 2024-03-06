using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class RegisterDeviceResponse
{
    public required DeviceId Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DeviceId CreatedByDevice { get; set; }
}
