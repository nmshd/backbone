using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("FinalizeExternalEventSyncSyncRunResponse")]
public class Response
{
    public long? NewDatawalletModificationIndex { get; set; }
    public required IEnumerable<CreatedDatawalletModificationDTO> DatawalletModifications { get; set; }
    public bool NewUnsyncedExternalEventsExist { get; set; }
}
