namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class SyncRun
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
    public required int EventCount { get; set; }
}
