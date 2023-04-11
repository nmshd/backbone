using Backbone.Modules.Devices.Application.Devices.DTOs;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

public class DeleteDeviceCommand : IRequest
{
    public DeviceId DeviceId { get; set; }
    public byte[] DeletionCertificate { get; set; }
    public SignedChallengeDTO SignedChallenge { get; set; }
}
