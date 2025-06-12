using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteExternalEventsOfIdentity;

public class DeleteExternalEventsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
