namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;

public class FinalizeExternalEventSyncResponse
{
    public long? NewDatawalletModificationIndex { get; set; }
    public required List<CreatedDatawalletModification> DatawalletModifications { get; set; }
    public bool NewUnsyncedExternalEventsExist { get; set; }
}
