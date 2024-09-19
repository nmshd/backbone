using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Common.Infrastructure.Persistence.Repository;

public interface IMetricStatusesRepository
{
    Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys);
}
