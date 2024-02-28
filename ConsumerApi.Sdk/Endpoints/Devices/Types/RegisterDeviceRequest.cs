namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;

public class RegisterDeviceRequest
{
    public required string DevicePassword { get; set; }
    public required SignedChallenge SignedChallenge { get; set; }
}
