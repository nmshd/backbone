namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class RegisterDeviceRequest
{
    public string? DevicePassword { get; set; }
    public RegisterDeviceRequestSignedChallenge? SignedChallenge { get; set; }
}

public class RegisterDeviceRequestSignedChallenge
{
    public string? Challenge { get; set; }
    public string? Signature { get; set; }
}
