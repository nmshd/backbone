using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentityQuotasByAddress;
public class GetIdentityQuotasByAddressResponse
{
    public GetIdentityQuotasByAddressResponse(IEnumerable<TierQuota> tierQuotas, IEnumerable<IndividualQuota> individualQuotas, IEnumerable<Metric> metrics)
    {
        TierQuotas = tierQuotas.Select(q =>
            new TierQuotaDTO(
                q.Id,
                new MetricDTO(metrics.First(m => m.Key == q.MetricKey)),
                q.Max,
                q.Period
            )
        );

        IndividualQuotas = individualQuotas.Select(q =>
            new IndividualQuotaDTO(
                q.Id,
                new MetricDTO(metrics.First(m => m.Key == q.MetricKey)),
                q.Max,
                q.Period
            )
        );
    }

    public IEnumerable<TierQuotaDTO> TierQuotas { get; set; }
    public IEnumerable<IndividualQuotaDTO> IndividualQuotas { get; set; }
}
