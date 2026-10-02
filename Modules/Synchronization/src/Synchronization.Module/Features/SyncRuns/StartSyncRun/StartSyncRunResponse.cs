using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;

public class StartSyncRunResponse
{
    public StartSyncRunResponse(StartSyncRunStatus status, SyncRun? newSyncRun = null)
    {
        Status = status;
        SyncRun = newSyncRun != null ? new SyncRunDTO(newSyncRun) : null;
    }

    public StartSyncRunStatus Status { get; set; }
    public SyncRunDTO? SyncRun { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StartSyncRunStatus
{
    Created,
    NoNewEvents
}
