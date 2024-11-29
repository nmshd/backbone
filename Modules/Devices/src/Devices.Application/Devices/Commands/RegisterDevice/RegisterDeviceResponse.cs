using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class RegisterDeviceResponse
{
    public RegisterDeviceResponse(Device device)
    {
        Id = device.Id.Value;
        Username = device.User.UserName!;
        CreatedByDevice = device.CreatedByDevice.Value;
        CreatedAt = device.CreatedAt;
        IsBackupDevice = device.IsBackupDevice;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByDevice { get; set; }
    public bool IsBackupDevice { get; set; }
}
