using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class FinalizeDatawalletVersionUpgradeSyncRunCommand : IRequest<FinalizeDatawalletVersionUpgradeSyncRunResponse>
{
    public required string SyncRunId { get; init; }
    public required ushort NewDatawalletVersion { get; init; }
    public required List<PushDatawalletModificationItem> DatawalletModifications { get; init; }
}
