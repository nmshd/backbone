using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeDatawalletVersionUpgrade;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("FinalizeDatawalletVersionUpgradeSyncRunResponse")]
public class Response
{
    public long? NewDatawalletModificationIndex { get; set; }

    public required IEnumerable<CreatedDatawalletModificationDTO> DatawalletModifications { get; set; }
}
