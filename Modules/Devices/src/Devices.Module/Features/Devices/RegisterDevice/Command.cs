using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RegisterDeviceCommand")]
public class Command : IRequest<Response>
{
    public required string DevicePassword { get; init; }
    public required string CommunicationLanguage { get; init; }
    public required SignedChallengeDTO SignedChallenge { get; init; }
    public required bool IsBackupDevice { get; init; }
}
