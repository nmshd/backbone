using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Devices.DTOs;

public class DeviceDTO
{
    public DeviceDTO(Device device)
    {
        Id = device.Id;
        Username = device.User.UserName!;
        CreatedAt = device.CreatedAt;
        CreatedByDevice = device.CreatedByDevice;
        LastLogin = new LastLoginInformation { Time = device.User.LastLoginAt };
        CommunicationLanguage = device.CommunicationLanguage;
        IsBackupDevice = device.IsBackupDevice;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByDevice { get; set; }
    public LastLoginInformation LastLogin { get; set; }
    public string CommunicationLanguage { get; set; }
    public bool IsBackupDevice { get; set; }
}

public class LastLoginInformation
{
    public DateTime? Time { get; set; }
}
