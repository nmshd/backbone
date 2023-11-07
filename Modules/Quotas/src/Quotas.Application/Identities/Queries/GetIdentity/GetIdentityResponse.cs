using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity;
public class GetIdentityResponse
{
    public string Address { get; set; }
    public IEnumerable<QuotaDTO> Quotas { get; set; }
}

public class GetIdentityResponseBuilder
{
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public GetIdentityResponseBuilder(MetricCalculatorFactory metricCalculatorFactory)
    {
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task<GetIdentityResponse> BuildResponse(string identityAddress, IEnumerable<TierQuota> tierQuotas, IEnumerable<IndividualQuota> individualQuotas, IEnumerable<Metric> metrics, CancellationToken cancellationToken)
    {
        var quotasList = new List<QuotaDTO>();

        var allQuotas = (individualQuotas as IEnumerable<Quota>).Union(tierQuotas);

        foreach (var quota in allQuotas)
        {
            var usage = await CalculateUsageAsync(_metricCalculatorFactory, quota, identityAddress, cancellationToken);
            quotasList.Add(new QuotaDTO(
                    quota.Id,
                    quota is TierQuota ? QuotaSource.Tier : QuotaSource.Individual,
                    new MetricDTO(metrics.First(m => m.Key == quota.MetricKey)),
                    usage,
                    quota.Max,
                    quota.Period.ToString()
                ));
        }

        return new GetIdentityResponse()
        {
            Address = identityAddress,
            Quotas = quotasList
        };
    }

    private static async Task<uint> CalculateUsageAsync(MetricCalculatorFactory metricCalculatorFactory, Quota quota, string identityAddress, CancellationToken cancellationToken)
    {
        var calculator = metricCalculatorFactory.CreateFor(quota.MetricKey);
        return await calculator.CalculateUsage(quota.Period.CalculateBegin(), quota.Period.CalculateEnd(), identityAddress, cancellationToken);
    }
}
