using Backbone.Modules.Devices.Application.Devices.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class RegisterDeviceCommand : IRequest<RegisterDeviceResponse>
{
    public required string DevicePassword { get; set; }
    public required SignedChallengeDTO SignedChallenge { get; set; }
}
