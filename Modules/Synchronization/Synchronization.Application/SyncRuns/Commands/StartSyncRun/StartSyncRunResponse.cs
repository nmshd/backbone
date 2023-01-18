using System.Text.Json.Serialization;
using Synchronization.Application.SyncRuns.DTOs;

namespace Synchronization.Application.SyncRuns.Commands.StartSyncRun;

public class StartSyncRunResponse
{
    public StartSyncRunStatus Status { get; set; }
    public SyncRunDTO SyncRun { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StartSyncRunStatus
{
    Created,
    NoNewEvents
}
