using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class Handler : IRequestHandler<GetSyncRunByIdQuery, SyncRunDTO>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _activeIdentity = userContext.GetAddress();
        userContext.GetDeviceId();
    }

    public async Task<SyncRunDTO> Handle(GetSyncRunByIdQuery request, CancellationToken cancellationToken)
    {
        var syncRun = await _dbContext.GetSyncRunAsNoTracking(SyncRunId.Parse(request.SyncRunId), _activeIdentity, CancellationToken.None);
        return new SyncRunDTO(syncRun);
    }
}
