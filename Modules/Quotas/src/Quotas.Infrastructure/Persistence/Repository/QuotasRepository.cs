using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class QuotasRepository : IQuotasRepository
{
    public Task<IEnumerable<Quota>> FindQuotasByMetricKey(string metricKey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
