using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IQuotasRepository
{
    Task<IEnumerable<Quota>> FindQuotasByMetricKey(string metricKey, CancellationToken cancellationToken);
}