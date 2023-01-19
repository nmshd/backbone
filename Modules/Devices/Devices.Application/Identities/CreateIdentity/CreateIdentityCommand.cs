using Devices.Application.Devices.DTOs;
using MediatR;

namespace Devices.Application.Identities.CreateIdentity;

public class CreateIdentityCommand : IRequest<CreateIdentityResponse>
{
    public string ClientId { get; set; }
    public byte[] IdentityPublicKey { get; set; }
    public string DevicePassword { get; set; }
    public byte IdentityVersion { get; set; }
    public SignedChallengeDTO SignedChallenge { get; set; }
}
