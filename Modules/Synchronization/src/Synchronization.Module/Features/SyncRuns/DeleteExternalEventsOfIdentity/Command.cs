using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteExternalEventsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteExternalEventsOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
