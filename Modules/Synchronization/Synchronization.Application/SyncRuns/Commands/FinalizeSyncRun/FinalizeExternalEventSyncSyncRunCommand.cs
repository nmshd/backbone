using System.Text.Json.Serialization;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using MediatR;
using Synchronization.Application.Datawallets.DTOs;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeExternalEventSyncSyncRunCommand : IRequest<FinalizeExternalEventSyncSyncRunResponse>
{
    public FinalizeExternalEventSyncSyncRunCommand(SyncRunId syncRunId) : this(syncRunId, new List<ExternalEventResult>(), new List<PushDatawalletModificationItem>()) { }

    public FinalizeExternalEventSyncSyncRunCommand(SyncRunId syncRunId, List<ExternalEventResult> externalEventResults) : this(syncRunId, externalEventResults, new List<PushDatawalletModificationItem>()) { }

    public FinalizeExternalEventSyncSyncRunCommand(SyncRunId syncRunId, List<PushDatawalletModificationItem> datawalletModifications) : this(syncRunId, new List<ExternalEventResult>(), datawalletModifications) { }

    [JsonConstructor]
    public FinalizeExternalEventSyncSyncRunCommand(SyncRunId syncRunId, List<ExternalEventResult> externalEventResults, List<PushDatawalletModificationItem> datawalletModifications)
    {
        SyncRunId = syncRunId;
        ExternalEventResults = externalEventResults;
        DatawalletModifications = datawalletModifications;
    }

    public SyncRunId SyncRunId { get; set; }
    public List<ExternalEventResult> ExternalEventResults { get; set; }
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; }

    public class ExternalEventResult : IMapTo<Domain.Entities.Sync.ExternalEventResult>
    {
        public ExternalEventResult(ExternalEventId externalEventId) : this(externalEventId, null) { }

        [JsonConstructor]
        public ExternalEventResult(ExternalEventId externalEventId, string errorCode)
        {
            ExternalEventId = externalEventId;
            ErrorCode = errorCode;
        }

        public ExternalEventId ExternalEventId { get; set; }
        public string ErrorCode { get; set; }
    }
}
