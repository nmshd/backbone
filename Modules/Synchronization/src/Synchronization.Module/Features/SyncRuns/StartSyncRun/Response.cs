using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("StartSyncRunResponse")]
public class Response
{
    public Response(StartSyncRunStatus status, SyncRun? newSyncRun = null)
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
