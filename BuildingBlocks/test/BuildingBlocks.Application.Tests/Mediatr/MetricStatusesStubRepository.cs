using Backbone.BuildingBlocks.Domain;
using Backbone.Common.Infrastructure.Persistence.Repository;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.Tests.Mediatr;

public class MetricStatusesStubRepository : IMetricStatusesRepository
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public MetricStatusesStubRepository(List<MetricStatus>? metricStatuses)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        if (metricStatuses != null)
        {
            MetricStatuses = metricStatuses;
        }
    }
    public List<MetricStatus> MetricStatuses { get; } = [];

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        return Task.FromResult(MetricStatuses.AsEnumerable());
    }
}
