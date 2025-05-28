using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.ListExternalEventsOfSyncRun;

public class Handler : IRequestHandler<ListExternalEventsOfSyncRunQuery, ListExternalEventsOfSyncRunResponse>
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

    public async Task<ListExternalEventsOfSyncRunResponse> Handle(ListExternalEventsOfSyncRunQuery request, CancellationToken cancellationToken)
    {
        var syncRun = await _dbContext.GetSyncRunAsNoTracking(SyncRunId.Parse(request.SyncRunId), _activeIdentity, cancellationToken);

        if (syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());

        if (syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotReadExternalEventsOfSyncRunStartedByAnotherDevice());

        var dbPaginationResult = await _dbContext.GetExternalEventsOfSyncRun(request.PaginationFilter, _activeIdentity, syncRun.Id, cancellationToken);

        return new ListExternalEventsOfSyncRunResponse(dbPaginationResult.ItemsOnPage.Select(e => new ExternalEventDTO(e)), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
