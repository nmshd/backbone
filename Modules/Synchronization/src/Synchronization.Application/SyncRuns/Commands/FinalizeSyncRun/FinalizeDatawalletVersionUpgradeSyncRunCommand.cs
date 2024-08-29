using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeDatawalletVersionUpgradeSyncRunCommand : IRequest<FinalizeDatawalletVersionUpgradeSyncRunResponse>
{
    [JsonConstructor]
    public FinalizeDatawalletVersionUpgradeSyncRunCommand(string syncRunId, ushort newDatawalletVersion, List<PushDatawalletModificationItem> datawalletModifications)
    {
        SyncRunId = syncRunId;
        NewDatawalletVersion = newDatawalletVersion;
        DatawalletModifications = datawalletModifications;
    }

    public string SyncRunId { get; set; }
    public ushort NewDatawalletVersion { get; set; }
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; }
}
