using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Identities.Commands.DeleteIdentity;
public class Handler : IRequestHandler<DeleteIdentityCommand>
{
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        await _dbContext.Set<Datawallet>().Where(d => d.Owner == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
        await _dbContext.Set<SyncRun>().Where(d => d.CreatedBy == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
