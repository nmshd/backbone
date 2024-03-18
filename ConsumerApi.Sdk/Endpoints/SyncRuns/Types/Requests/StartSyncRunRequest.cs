namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;

public class StartSyncRunRequest
{
    public required SyncRunType Type { get; set; }
    public ushort? Duration { get; set; }
}

public enum SyncRunType
{
    ExternalEventSync = 0,
    DatawalletVersionUpgrade = 1
}
