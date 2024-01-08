using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsOfIdentity;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Identities;
public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;

    public IdentityDeleter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteSyncRunsOfIdentityCommand(identityAddress));
        await _mediator.Send(new DeleteDatawalletsOfIdentityCommand(identityAddress));
    }
}
