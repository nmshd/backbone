using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeExternalEventSyncSyncRunCommand : IRequest<FinalizeExternalEventSyncSyncRunResponse>
{
    public required string SyncRunId { get; init; }
    public List<ExternalEventResult> ExternalEventResults { get; set; } = [];
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];

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
