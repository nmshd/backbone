using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;
public class Handler(ISynchronizationDbContext dbContext) : IRequestHandler<DeleteSyncRunsOfIdentityCommand>
{
    public async Task Handle(DeleteSyncRunsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Set<ExternalEvent>().Where(d => d.Owner == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
        await dbContext.Set<SyncRun>().Where(d => d.CreatedBy == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
