using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsByIdentity;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsByIdentity;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Identities;
public class IdentityDeleter(IMediator mediator) : IIdentityDeleter
{
    private readonly IMediator _mediator = mediator;

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteSyncRunsByIdentityCommand(identityAddress));
        await _mediator.Send(new DeleteDatawalletsByIdentityCommand(identityAddress));
    }
}
