namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class CreateIdentityRequest
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public required byte IdentityVersion { get; set; }
    public required CreateIdentityRequestSignedChallenge SignedChallenge { get; set; }
}

public class CreateIdentityRequestSignedChallenge
{
    public required string Challenge { get; set; }
    public required string Signature { get; set; }
}
