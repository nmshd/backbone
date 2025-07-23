using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class Handler : IRequestHandler<RefreshExpirationTimeOfSyncRunCommand, RefreshExpirationTimeOfSyncRunResponse>
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

    public async Task<RefreshExpirationTimeOfSyncRunResponse> Handle(RefreshExpirationTimeOfSyncRunCommand request, CancellationToken cancellationToken)
    {
        var syncRun = await _dbContext.GetSyncRun(SyncRunId.Parse(request.SyncRunId), _activeIdentity, cancellationToken);

        CheckPrerequisites(syncRun);

        syncRun.RefreshExpirationTime();

        await SaveSyncRun(syncRun, cancellationToken);

        return new RefreshExpirationTimeOfSyncRunResponse { ExpiresAt = syncRun.ExpiresAt };
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
        _dbContext.Set<SyncRun>().Entry(syncRun).CurrentValues.SetValues(syncRun);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
