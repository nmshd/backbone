using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.StartedDeletionProcesses;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class StartedDeletionProcessesRepository : IStartedDeletionProcessesRepository
{
    private readonly IQueryable<StartedDeletionProcess> _readOnlyStartedDeletionProcesses;
    public StartedDeletionProcessesRepository(QuotasDbContext dbContext)
    {
        _readOnlyStartedDeletionProcesses = dbContext.StartedDeletionProcesses.AsNoTracking();
    }

    public Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        //var count = await _readOnlyStartedDeletionProcesses.
        //    .CreatedInInterval(createdAtFrom, createdAtTo)
        //    .CountAsync(p => p.)
        throw new NotImplementedException();
    }
}
