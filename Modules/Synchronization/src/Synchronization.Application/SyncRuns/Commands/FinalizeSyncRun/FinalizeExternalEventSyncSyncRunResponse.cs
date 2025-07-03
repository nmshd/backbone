using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeExternalEventSyncSyncRunResponse
{
    public long? NewDatawalletModificationIndex { get; set; }
    public required IEnumerable<CreatedDatawalletModificationDTO> DatawalletModifications { get; set; }
    public bool NewUnsyncedExternalEventsExist { get; set; }
}
