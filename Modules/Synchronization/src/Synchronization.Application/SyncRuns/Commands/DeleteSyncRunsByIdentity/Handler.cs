using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsByIdentity;
public class Handler(ISynchronizationDbContext dbContext) : IRequestHandler<DeleteSyncRunsByIdentityCommand>
{
    public async Task Handle(DeleteSyncRunsByIdentityCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Set<ExternalEvent>().Where(d => d.Owner == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
        await dbContext.Set<SyncRun>().Where(d => d.CreatedBy == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
