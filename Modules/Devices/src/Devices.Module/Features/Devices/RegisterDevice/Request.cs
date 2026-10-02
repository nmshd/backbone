using Backbone.Modules.Devices.Module.Features.Devices.Shared;

namespace Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;

public class RegisterDeviceRequest
{
    public required string DevicePassword { get; set; }
    public string? CommunicationLanguage { get; set; }
    public required SignedChallengeDTO SignedChallenge { get; set; }
    public bool? IsBackupDevice { get; set; }
}
