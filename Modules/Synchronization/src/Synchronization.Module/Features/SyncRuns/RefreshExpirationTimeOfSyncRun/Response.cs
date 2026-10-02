namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RefreshExpirationTimeOfSyncRunResponse")]
public class Response
{
    public DateTime ExpiresAt { get; set; }
}
