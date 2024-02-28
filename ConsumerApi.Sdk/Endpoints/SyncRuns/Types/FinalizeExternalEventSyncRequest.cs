using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;
public class FinalizeExternalEventSyncRequest
{
    public required List<ExternalEventResult> ExternalEventResults { get; set; }
    public required List<PushDatawalletModificationItem> DatawalletModifications { get; set; }
}
