using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;

namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;

public class FinalizeExternalEventSyncRequest
{
    public required List<ExternalEventResult> ExternalEventResults { get; set; }
    public required List<PushDatawalletModificationsRequestItem> DatawalletModifications { get; set; }
}
