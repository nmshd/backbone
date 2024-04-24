using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;

public class Handler : IRequestHandler<DeleteSyncRunsOfIdentityCommand>
{
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteSyncRunsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _dbContext.Set<SyncRun>().Where(d => d.CreatedBy == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
