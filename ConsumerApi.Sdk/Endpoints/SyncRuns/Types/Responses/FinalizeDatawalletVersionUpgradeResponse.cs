namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;

public class FinalizeDatawalletVersionUpgradeResponse
{
    public long? NewDatawalletModificationIndex { get; set; }

    public required List<CreatedDatawalletModification> DatawalletModifications { get; set; }
}
