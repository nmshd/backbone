namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class StartSyncRunRequest
{
    public string? Type { get; set; }
    public ushort? Duration { get; set; }
}
