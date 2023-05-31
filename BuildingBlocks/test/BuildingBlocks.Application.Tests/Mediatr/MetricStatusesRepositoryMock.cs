using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

public class MockMetricStatusesRepository : IMetricStatusesRepository
{
    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        return Task.FromResult(
            new List<MetricStatus> {
                new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(-1)),
                new MetricStatus(new MetricKey("KeyTwo"), DateTime.UtcNow.AddDays(1)),
                new MetricStatus(new MetricKey("KeyThree"), DateTime.UtcNow.AddDays(1)),
            }.AsEnumerable());
    }
}
