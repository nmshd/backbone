using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;

public class StartSyncRunResponse
{
    public required StartSyncRunStatus Status { get; set; }
    public required SyncRunDTO SyncRun { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StartSyncRunStatus
{
    Created,
    NoNewEvents
}
