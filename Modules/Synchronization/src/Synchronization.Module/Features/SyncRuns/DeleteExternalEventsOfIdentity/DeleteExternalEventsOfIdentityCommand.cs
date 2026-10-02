using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteExternalEventsOfIdentity;

public class DeleteExternalEventsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
