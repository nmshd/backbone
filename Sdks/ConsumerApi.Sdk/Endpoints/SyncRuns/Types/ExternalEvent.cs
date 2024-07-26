namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class ExternalEvent
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required byte SyncErrorCount { get; set; }
    public required object Payload { get; set; }
}
