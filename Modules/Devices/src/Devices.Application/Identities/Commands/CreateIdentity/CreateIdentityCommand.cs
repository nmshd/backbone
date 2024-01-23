using Backbone.Modules.Devices.Application.Devices.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

public class CreateIdentityCommand : IRequest<CreateIdentityResponse>
{
    public string? ClientId { get; set; }
    public required byte[] IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public required byte IdentityVersion { get; set; }
    public required SignedChallengeDTO SignedChallenge { get; set; }
}
