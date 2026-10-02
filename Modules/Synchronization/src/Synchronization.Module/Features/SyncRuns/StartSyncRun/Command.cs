using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("StartSyncRunCommand")]
public class Command : IRequest<Response>
{
    public required SyncRunDTO.SyncRunType Type { get; init; }
    public ushort? Duration { get; init; }
    public required ushort SupportedDatawalletVersion { get; init; }
}
