using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;

public class RegisterDeviceCommand : IRequest<RegisterDeviceResponse>
{
    public required string DevicePassword { get; init; }
    public required string CommunicationLanguage { get; init; }
    public required SignedChallengeDTO SignedChallenge { get; init; }
    public required bool IsBackupDevice { get; init; }
}
