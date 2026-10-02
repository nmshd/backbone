using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.CreateIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateIdentityCommand")]
public class Command : IRequest<Response>
{
    public required string ClientId { get; set; }
    public required byte[] IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public required string CommunicationLanguage { get; set; }
    public required byte IdentityVersion { get; set; }
    public required SignedChallengeDTO SignedChallenge { get; set; }
}
