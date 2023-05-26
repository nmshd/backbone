using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Enmeshed.Common.Infrastructure.Persistence.Repository;

public class MetricStatusRepository<TDbContext> : IMetricStatusesRepository where TDbContext : AbstractDbContextBase
{
    protected readonly DbSet<MetricStatus> _dbSet;

    public MetricStatusRepository(TDbContext dbContext) 
    {
        _dbSet = dbContext.Set<MetricStatus>();
    }

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        throw new NotImplementedException();
    }
}
