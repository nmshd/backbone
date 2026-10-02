using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteExternalEventsOfIdentity;

public class Handler : IRequestHandler<Command>
{
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _dbContext.Set<ExternalEvent>().Where(d => d.Owner == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
