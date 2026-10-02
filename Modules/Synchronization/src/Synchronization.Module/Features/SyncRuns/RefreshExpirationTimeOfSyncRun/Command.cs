using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RefreshExpirationTimeOfSyncRunCommand")]
public class Command : IRequest<Response>
{
    public required string SyncRunId { get; init; }
}
