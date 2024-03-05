using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class Handler : IRequestHandler<GetSyncRunByIdQuery, SyncRunDTO>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
        userContext.GetDeviceId();
    }

    public async Task<SyncRunDTO> Handle(GetSyncRunByIdQuery request, CancellationToken cancellationToken)
    {
        var syncRun = await _dbContext.GetSyncRunAsNoTracking(request.SyncRunId, _activeIdentity, CancellationToken.None);

        var dto = _mapper.Map<SyncRunDTO>(syncRun);

        return dto;
    }
}
