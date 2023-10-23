using Backbone.Synchronization.Application.Datawallets.DTOs;

namespace Backbone.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeExternalEventSyncSyncRunResponse
{
    public long? NewDatawalletModificationIndex { get; set; }

    public IEnumerable<CreatedDatawalletModificationDTO> DatawalletModifications { get; set; }
}
