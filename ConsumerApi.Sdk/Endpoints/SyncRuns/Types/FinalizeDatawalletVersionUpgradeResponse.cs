namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class FinalizeDatawalletVersionUpgradeResponse
{
    public long? NewDatawalletModificationIndex { get; set; }

    public required List<CreatedDatawalletModification> DatawalletModifications { get; set; }
}
