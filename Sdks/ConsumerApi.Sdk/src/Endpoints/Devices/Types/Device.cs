namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;

public class Device
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedByDevice { get; set; }
    public required LastLoginInformation LastLogin { get; set; }
    public required string CommunicationLanguage { get; set; }
    public required bool IsBackupDevice { get; set; }
}

public class LastLoginInformation
{
    public DateTime? Time { get; set; }
}
