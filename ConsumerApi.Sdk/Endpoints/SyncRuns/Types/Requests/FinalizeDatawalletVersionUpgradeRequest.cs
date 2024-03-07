using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;

namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;

public class FinalizeDatawalletVersionUpgradeRequest
{
    public required ushort NewDatawalletVersion { get; set; }
    public List<PushDatawalletModificationsRequestItem> DatawalletModifications { get; set; } = [];
}
