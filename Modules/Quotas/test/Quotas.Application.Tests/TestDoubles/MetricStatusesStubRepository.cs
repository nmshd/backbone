using Backbone.BuildingBlocks.Domain;
using Backbone.Common.Infrastructure.Persistence.Repository;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;

internal class MetricStatusesStubRepository : IMetricStatusesRepository
{
    private readonly List<MetricStatus> _metricStatuses;

    public MetricStatusesStubRepository(List<MetricStatus> metricStatuses)
    {
        _metricStatuses = metricStatuses;
    }

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        return Task.FromResult(_metricStatuses.AsEnumerable());
    }
}
