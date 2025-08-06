using Backbone.Modules.Devices.Application.Devices.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class RegisterDeviceCommand : IRequest<RegisterDeviceResponse>
{
    public required string DevicePassword { get; init; }
    public required string CommunicationLanguage { get; init; }
    public required SignedChallengeDTO SignedChallenge { get; init; }
    public required bool IsBackupDevice { get; init; }
}
