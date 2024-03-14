namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;

public class SignedChallenge
{
    public required string Challenge { get; set; }
    public required byte[] Signature { get; set; }
}
