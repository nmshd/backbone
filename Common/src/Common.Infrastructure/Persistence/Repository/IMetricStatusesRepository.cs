using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Enmeshed.Common.Infrastructure.Persistence.Repository;
public interface IMetricStatusesRepository
{
    Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys);
}
