using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;

public class FinalizeExternalEventSyncSyncRunResponse
{
    public long? NewDatawalletModificationIndex { get; set; }
    public required IEnumerable<CreatedDatawalletModificationDTO> DatawalletModifications { get; set; }
    public bool NewUnsyncedExternalEventsExist { get; set; }
}
