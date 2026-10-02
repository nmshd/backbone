using DeleteDatawalletsOfIdentity = Backbone.Modules.Synchronization.Module.Features.Datawallets.DeleteDatawalletsOfIdentity;
using DeleteExternalEventsOfIdentity = Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteExternalEventsOfIdentity;
using DeleteSyncRunsOfIdentity = Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteSyncRunsOfIdentity;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Identities.DeleteIdentity;

public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;
    private readonly IDeletionProcessLogger _deletionProcessLogger;

    public IdentityDeleter(IMediator mediator, IDeletionProcessLogger deletionProcessLogger)
    {
        _mediator = mediator;
        _deletionProcessLogger = deletionProcessLogger;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteExternalEventsOfIdentity.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "ExternalEvents");
        await _mediator.Send(new DeleteSyncRunsOfIdentity.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "SyncRuns");
        await _mediator.Send(new DeleteDatawalletsOfIdentity.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "Datawallets");
    }
}
