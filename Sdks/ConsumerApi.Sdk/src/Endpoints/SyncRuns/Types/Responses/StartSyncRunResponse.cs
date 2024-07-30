namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;

public class StartSyncRunResponse
{
    public required string Status { get; set; }
    public required SyncRun SyncRun { get; set; }
}
