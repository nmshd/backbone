using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;

public class Handler : IRequestHandler<GetExternalEventsOfSyncRunQuery, GetExternalEventsOfSyncRunResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(ISynchronizationDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<GetExternalEventsOfSyncRunResponse> Handle(GetExternalEventsOfSyncRunQuery request, CancellationToken cancellationToken)
    {
        var syncRun = await _dbContext.GetSyncRunAsNoTracking(request.SyncRunId, _activeIdentity, cancellationToken);

        if (syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());

        if (syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotReadExternalEventsOfSyncRunStartedByAnotherDevice());

        var dbPaginationResult = await _dbContext.GetExternalEventsOfSyncRun(request.PaginationFilter, _activeIdentity, syncRun.Id, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<ExternalEventDTO>>(dbPaginationResult.ItemsOnPage);

        return new GetExternalEventsOfSyncRunResponse(dtos, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
