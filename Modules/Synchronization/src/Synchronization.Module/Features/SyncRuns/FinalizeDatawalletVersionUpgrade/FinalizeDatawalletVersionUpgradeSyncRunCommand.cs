using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeDatawalletVersionUpgrade;

public class FinalizeDatawalletVersionUpgradeSyncRunCommand : IRequest<FinalizeDatawalletVersionUpgradeSyncRunResponse>
{
    public required string SyncRunId { get; init; }
    public required ushort NewDatawalletVersion { get; init; }
    public required List<PushDatawalletModificationItem> DatawalletModifications { get; init; }
}
