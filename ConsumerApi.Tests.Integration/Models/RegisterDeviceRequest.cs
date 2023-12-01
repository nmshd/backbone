namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class RegisterDeviceRequest
{
    public required string DevicePassword { get; set; }
    public RegisterDeviceRequestSignedChallenge? SignedChallenge { get; set; }
}

public class RegisterDeviceRequestSignedChallenge
{
    public required string Challenge { get; set; }
    public required string Signature { get; set; }
}
