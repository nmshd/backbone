namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;

public class RegisterDeviceResponse
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedByDevice { get; set; }
    public required bool IsBackupDevice { get; set; }
}
