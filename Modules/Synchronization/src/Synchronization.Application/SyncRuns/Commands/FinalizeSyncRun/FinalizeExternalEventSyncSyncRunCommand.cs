using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeExternalEventSyncSyncRunCommand : IRequest<FinalizeExternalEventSyncSyncRunResponse>
{
    public FinalizeExternalEventSyncSyncRunCommand(string syncRunId) : this(syncRunId, [], [])
    {
    }

    public FinalizeExternalEventSyncSyncRunCommand(string syncRunId, List<ExternalEventResult> externalEventResults) : this(syncRunId, externalEventResults, [])
    {
    }

    public FinalizeExternalEventSyncSyncRunCommand(string syncRunId, List<PushDatawalletModificationItem> datawalletModifications) : this(syncRunId, [], datawalletModifications)
    {
    }

    [JsonConstructor]
    public FinalizeExternalEventSyncSyncRunCommand(string syncRunId, List<ExternalEventResult> externalEventResults, List<PushDatawalletModificationItem> datawalletModifications)
    {
        SyncRunId = syncRunId;
        ExternalEventResults = externalEventResults;
        DatawalletModifications = datawalletModifications;
    }

    public string SyncRunId { get; set; }
    public List<ExternalEventResult> ExternalEventResults { get; set; }
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; }

    public class ExternalEventResult
    {
        [JsonConstructor]
        public ExternalEventResult(string externalEventId, string? errorCode = null)
        {
            ExternalEventId = externalEventId;
            ErrorCode = errorCode;
        }

        public string ExternalEventId { get; set; }
        public string? ErrorCode { get; set; }
    }
}
