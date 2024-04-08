using Backbone.BuildingBlocks.Domain;
using Backbone.Common.Infrastructure.Persistence.Repository;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;

internal class MetricStatusesStubRepository : IMetricStatusesRepository
{
    public MetricStatusesStubRepository(List<MetricStatus> metricStatuses)
    {
        MetricStatuses = metricStatuses;
    }

    public List<MetricStatus> MetricStatuses { get; }

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        return Task.FromResult(MetricStatuses.AsEnumerable());
    }
}
