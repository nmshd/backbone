﻿using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentityQuotasByAddress;
public class GetIdentityQuotasByAddressResponse
{
    public GetIdentityQuotasByAddressResponse(string identityAddress, IEnumerable<TierQuota> tierQuotas, IEnumerable<IndividualQuota> individualQuotas, IEnumerable<Metric> metrics)
    {
        var quotasList = new List<QuotaDTO>();
        quotasList.AddRange(individualQuotas.Select(q =>
            new QuotaDTO(
                q.Id,
                "Individual",
                new MetricDTO(metrics.First(m => m.Key == q.MetricKey)),
                q.Max,
                q.Period.ToString()
            )
        ));
        quotasList.AddRange(tierQuotas.Select(q =>
            new QuotaDTO(
                q.Id,
                "Tier",
                new MetricDTO(metrics.First(m => m.Key == q.MetricKey)),
                q.Max,
                q.Period.ToString()
            )
        ));

        Address = identityAddress;
        Quotas = quotasList;
    }

    public string Address { get; set; }
    public IEnumerable<QuotaDTO> Quotas { get; set; }
}
