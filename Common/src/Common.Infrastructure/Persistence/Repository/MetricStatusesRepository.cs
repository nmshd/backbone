using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Common.Infrastructure.Persistence.Context;

namespace Enmeshed.Common.Infrastructure.Persistence.Repository;

public class MetricStatusesRepository : IMetricStatusesRepository
{
    private readonly DapperContext _context;

    public MetricStatusesRepository(DapperContext context) 
    {
        _context = context;
    }

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        throw new NotImplementedException();
    }
}
