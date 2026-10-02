using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;

public class Command : IRequest<Response>
{
    public required SyncRunDTO.SyncRunType Type { get; init; }
    public ushort? Duration { get; init; }
    public required ushort SupportedDatawalletVersion { get; init; }
}
