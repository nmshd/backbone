using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Module.Features.Devices.Shared;

public class DeviceDTO
{
    public DeviceDTO(Device device)
    {
        Id = device.Id;
        Username = device.Username;
        CreatedAt = device.CreatedAt;
        CreatedByDevice = device.CreatedByDevice;
        LastLogin = device.User.LastLoginAt == null ? null : new LastLoginInformation { Time = device.User.LastLoginAt.Value };
        CommunicationLanguage = device.CommunicationLanguage;
        IsBackupDevice = device.IsBackupDevice;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByDevice { get; set; }
    public LastLoginInformation? LastLogin { get; set; }
    public string CommunicationLanguage { get; set; }
    public bool IsBackupDevice { get; set; }
}

public class LastLoginInformation
{
    public DateTime Time { get; set; }
}
