using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;

public class CreateIdentityRequest
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public required byte IdentityVersion { get; set; }
    public required SignedChallenge SignedChallenge { get; set; }
}
