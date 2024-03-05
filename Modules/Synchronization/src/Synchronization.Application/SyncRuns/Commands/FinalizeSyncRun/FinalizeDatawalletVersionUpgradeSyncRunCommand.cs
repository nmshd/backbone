using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

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
