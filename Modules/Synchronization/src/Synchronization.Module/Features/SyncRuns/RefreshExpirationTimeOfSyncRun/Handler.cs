using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;

public class Handler : IRequestHandler<Command, Response>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var syncRun = await _dbContext.GetSyncRun(SyncRunId.Parse(request.SyncRunId), _activeIdentity, cancellationToken);

        CheckPrerequisites(syncRun);

        syncRun.RefreshExpirationTime();

        await SaveSyncRun(syncRun, cancellationToken);

        return new Response { ExpiresAt = syncRun.ExpiresAt };
    }

    private void CheckPrerequisites(SyncRun syncRun)
    {
        if (syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotRefreshExpirationTimeOfSyncRunStartedByAnotherDevice());

        if (syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());
    }

    private async Task SaveSyncRun(SyncRun syncRun, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
