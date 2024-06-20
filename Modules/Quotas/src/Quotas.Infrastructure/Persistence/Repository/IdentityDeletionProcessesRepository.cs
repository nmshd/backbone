using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class IdentityDeletionProcessesRepository : IIdentityDeletionProcessesRepository
{
    private readonly IQueryable<IdentityDeletionProcesses> _readOnlyIdentityDeletionProcesses;

    public IdentityDeletionProcessesRepository(QuotasDbContext dbContext)
    {
        _readOnlyIdentityDeletionProcesses = dbContext.IdentityDeletionProcesses.AsNoTracking();
    }

    public async Task<uint> CountInStatus(string createdBy, DateTime createdAtFrom, DateTime createdAtTo, DeletionProcessStatus status, CancellationToken cancellationToken)
    {
        var count = await _readOnlyIdentityDeletionProcesses
            .CreatedInInterval(createdAtFrom, createdAtTo)
            .CountAsync(p => p.Status == status, cancellationToken);

        return (uint)count;
    }
}
