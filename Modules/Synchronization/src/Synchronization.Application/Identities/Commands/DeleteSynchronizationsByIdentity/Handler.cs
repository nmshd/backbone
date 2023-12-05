using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Identities.Commands.DeleteSynchronizationsByIdentity;
public class Handler(ISynchronizationDbContext dbContext) : IRequestHandler<DeleteSynchronizationsByIdentity>
{
    private readonly ISynchronizationDbContext _dbContext = dbContext;

    public async Task Handle(DeleteSynchronizationsByIdentity request, CancellationToken cancellationToken)
    {
        await _dbContext.Set<Datawallet>().Where(d => d.Owner == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
        await _dbContext.Set<SyncRun>().Where(d => d.CreatedBy == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
