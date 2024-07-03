using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.DatawalletModifications;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class DatawalletModificationsRepository : IDatawalletModificationsRepository
{
    private readonly IQueryable<DatawalletModification> _readOnlyDatawalletModifications;

    public DatawalletModificationsRepository(QuotasDbContext dbContext)
    {
        _readOnlyDatawalletModifications = dbContext.DatawalletModifications.AsNoTracking();
    }

    public async Task<uint> Count(string identityAddress, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var count = await _readOnlyDatawalletModifications
            .CreatedInInterval(from, to)
            .CountAsync(d => d.CreatedBy == identityAddress, cancellationToken);

        return (uint)count;
    }
}
