using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteExternalEventsOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
