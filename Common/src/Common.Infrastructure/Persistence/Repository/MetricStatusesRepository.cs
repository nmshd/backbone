using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Common.Infrastructure.Persistence.Context;
using Dapper;

namespace Enmeshed.Common.Infrastructure.Persistence.Repository;

public class MetricStatusesRepository : IMetricStatusesRepository
{
    public MetricStatusesRepository(MetricStatusesDapperContext context)
    {
        _context = context;
    }

    private readonly MetricStatusesDapperContext _context;

    public async Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identityAddress, IEnumerable<MetricKey> keys)
    {
        const string QUERY = "SELECT * FROM MetricStatus WHERE Owner = @identityAddress and MetricKey IN @keys";
        using var connection = _context.Connection;
        var metricStatuses = await connection.QueryAsync<MetricStatus>(QUERY, new
        {
            identityAddress = identityAddress.ToString(),
            keys = keys.Select(x => x.Value)
        });
        return metricStatuses.ToList();
    }
}
