using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;

public class StartSyncRunCommand : IRequest<StartSyncRunResponse>
{
    public required SyncRunDTO.SyncRunType Type { get; init; }
    public ushort? Duration { get; init; }
    public required ushort SupportedDatawalletVersion { get; init; }
}
