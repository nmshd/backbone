using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class FinalizeDatawalletVersionUpgradeRequest
{
    public required ushort NewDatawalletVersion { get; set; }
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];
}
