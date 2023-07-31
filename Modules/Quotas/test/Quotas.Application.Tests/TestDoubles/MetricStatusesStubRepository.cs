using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Quotas.Application.Tests.TestDoubles;

internal class MetricStatusesStubRepository : IMetricStatusesRepository
{
    public MetricStatusesStubRepository(List<MetricStatus> metricStatuses)
    {
        if (metricStatuses != null)
        {
            MetricStatuses = metricStatuses;
        }
    }

    public List<MetricStatus> MetricStatuses { get; } = new();

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        return Task.FromResult(MetricStatuses.AsEnumerable());
    }
}
