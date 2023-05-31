using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Common.Infrastructure.Persistence.Context;
using Dapper;

namespace Enmeshed.Common.Infrastructure.Persistence.Repository;

public class MetricStatusesRepository : IMetricStatusesRepository
{
    private readonly MetricStatusesDapperContext _context;

    public MetricStatusesRepository(MetricStatusesDapperContext context) 
    {
        _context = context;
    }

    public async Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identityAddress, IEnumerable<MetricKey> keys)
    {
        /*var query = "SELECT * FROM MetricStatuses WHERE identityAddress = @identityAddress";
        using (var connection = _context.CreateConnection())
        {
            var metricStatuses = await connection.QueryAsync<MetricStatus>(query, new {identityAddress});
            return metricStatuses.ToList();
        }*/

        return new List<MetricStatus>
        {
            new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(1)),
            new MetricStatus(new MetricKey("KeyTwo"), DateTime.UtcNow.AddDays(1)),
            new MetricStatus(new MetricKey("KeyThree"), DateTime.UtcNow.AddDays(1)),
        };
    }
}
