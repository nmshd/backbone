namespace Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Requests;

public class UpdateDeviceRegistrationRequest
{
    public required string Platform { get; set; }
    public required string Handle { get; set; }
    public required string AppId { get; set; }
    public string? Environment { get; set; }
}
