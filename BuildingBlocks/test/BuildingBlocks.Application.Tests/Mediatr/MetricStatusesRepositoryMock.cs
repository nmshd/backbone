using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

public class MockMetricStatusesRepository : IMetricStatusesRepository
{
    public List<MetricStatus> MetricStatuses { get; } = new List<MetricStatus> {
                new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(-1)),
                new MetricStatus(new MetricKey("KeyTwo"), DateTime.UtcNow.AddDays(-2)),
                new MetricStatus(new MetricKey("KeyThree"), DateTime.UtcNow.AddDays(-3)),
            };

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public MockMetricStatusesRepository(List<MetricStatus>? metricStatuses)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        if (metricStatuses != null)
        {
            MetricStatuses = metricStatuses;
        }
    }

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        return Task.FromResult(MetricStatuses.AsEnumerable());
    }
}
