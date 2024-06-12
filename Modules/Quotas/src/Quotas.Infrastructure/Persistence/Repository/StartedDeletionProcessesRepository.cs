using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.StartedDeletionProcesses;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class StartedDeletionProcessesRepository : IStartedDeletionProcessesRepository
{
    private readonly IQueryable<IdentityDeletionProcesses> _readOnlyIdentityDeletionProcesses;
    public StartedDeletionProcessesRepository(QuotasDbContext dbContext)
    {
        _readOnlyIdentityDeletionProcesses = dbContext.StartedDeletionProcesses.AsNoTracking();
    }

    public async Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var count = await _readOnlyIdentityDeletionProcesses
            .CreatedInInterval(createdAtFrom, createdAtTo)
            .CountAsync(p => p.Status == DeletionProcessStatus.WaitingForApproval, cancellationToken);
        return (uint)count;
    }
}
