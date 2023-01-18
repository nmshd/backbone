using Synchronization.Application.Datawallets.DTOs;

namespace Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeDatawalletVersionUpgradeSyncRunResponse
{
    public long? NewDatawalletModificationIndex { get; set; }

    public IEnumerable<CreatedDatawalletModificationDTO> DatawalletModifications { get; set; }
}
