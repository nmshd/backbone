using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteSyncRunsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteSyncRunsOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
