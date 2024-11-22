using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class RegisterDeviceResponse
{
    public RegisterDeviceResponse(ApplicationUser user)
    {
        Id = user.DeviceId;
        Username = user.UserName!;
        CreatedByDevice = user.Device.CreatedByDevice;
        CreatedAt = user.Device.CreatedAt;
        IsBackupDevice = user.Device.IsBackupDevice;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByDevice { get; set; }
    public bool IsBackupDevice { get; set; }
}
