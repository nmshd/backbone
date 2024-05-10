using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeExternalEventSyncSyncRunCommand : IRequest<FinalizeExternalEventSyncSyncRunResponse>
{
    public FinalizeExternalEventSyncSyncRunCommand(SyncRunId syncRunId) : this(syncRunId, [], [])
    {
    }

    public FinalizeExternalEventSyncSyncRunCommand(SyncRunId syncRunId, List<ExternalEventResult> externalEventResults) : this(syncRunId, externalEventResults, [])
    {
    }

    public FinalizeExternalEventSyncSyncRunCommand(SyncRunId syncRunId, List<PushDatawalletModificationItem> datawalletModifications) : this(syncRunId, [], datawalletModifications)
    {
    }

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
        public ExternalEventResult(ExternalEventId externalEventId) : this(externalEventId, null)
        {
        }

        [JsonConstructor]
        public ExternalEventResult(ExternalEventId externalEventId, string? errorCode)
        {
            ExternalEventId = externalEventId;
            ErrorCode = errorCode;
        }

        public ExternalEventId ExternalEventId { get; set; }
        public string? ErrorCode { get; set; }
    }
}
