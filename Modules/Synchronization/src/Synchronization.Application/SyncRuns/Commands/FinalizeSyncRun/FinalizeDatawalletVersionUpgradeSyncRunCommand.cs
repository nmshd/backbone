using System.Text.Json.Serialization;
using MediatR;
using Synchronization.Application.Datawallets.DTOs;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeDatawalletVersionUpgradeSyncRunCommand : IRequest<FinalizeDatawalletVersionUpgradeSyncRunResponse>
{
    [JsonConstructor]
    public FinalizeDatawalletVersionUpgradeSyncRunCommand(SyncRunId syncRunId, ushort newDatawalletVersion, List<PushDatawalletModificationItem> datawalletModifications)
    {
        SyncRunId = syncRunId;
        NewDatawalletVersion = newDatawalletVersion;
        DatawalletModifications = datawalletModifications;
    }

    public SyncRunId SyncRunId { get; set; }
    public ushort NewDatawalletVersion { get; set; }
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; }
}
