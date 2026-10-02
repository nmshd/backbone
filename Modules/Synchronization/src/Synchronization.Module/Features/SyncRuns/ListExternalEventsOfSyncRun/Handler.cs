using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.ListExternalEventsOfSyncRun;

public class Handler : IRequestHandler<Query, Response>
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

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var syncRun = await _dbContext.GetSyncRunAsNoTracking(SyncRunId.Parse(request.SyncRunId), _activeIdentity, cancellationToken);

        if (syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());

        if (syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotReadExternalEventsOfSyncRunStartedByAnotherDevice());

        var dbPaginationResult = await _dbContext.GetExternalEventsOfSyncRun(request.PaginationFilter, _activeIdentity, syncRun.Id, cancellationToken);

        return new Response(dbPaginationResult.ItemsOnPage.Select(e => new ExternalEventDTO(e)), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
