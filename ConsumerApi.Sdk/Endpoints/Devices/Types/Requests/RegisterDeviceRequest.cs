namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;

public class RegisterDeviceRequest
{
    public required string DevicePassword { get; set; }
    public required SignedChallenge SignedChallenge { get; set; }
}
