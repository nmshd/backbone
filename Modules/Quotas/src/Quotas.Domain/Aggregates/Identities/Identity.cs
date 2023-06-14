using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.Tooling;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    private readonly List<TierQuota> _tierQuotas = new();

    public Identity(string address, TierId tierId)
    {
        Address = address;
        TierId = tierId;
    }

    public IReadOnlyCollection<TierQuota> TierQuotas => _tierQuotas;
    public string Address { get; }
    public TierId TierId { get; }

    public void AssignTierQuotaFromDefinition(TierQuotaDefinition definition)
    {
        var tierQuota = new TierQuota(definition, Address);
        _tierQuotas.Add(tierQuota);
    }

    public async Task UpdateMetrics(IEnumerable<string> metrics, IMetricCalculatorFactory factory, CancellationToken cancellationToken)
    {
        foreach (var metric in metrics)
        {
            var metricCalculator = factory.CreateFor(metric);
            await UpdateMetricAsync(metric, metricCalculator, cancellationToken);
        }
        return;
    }

    private async Task UpdateMetricAsync(string metric, IMetricCalculator metricCalculator, CancellationToken cancellationToken)
    {
        if (TierQuotas.Count == 0)
        {
            return;
        }

        var quotas = GetAppliedQuotasForMetricAsync(metric, TierQuotas);
        var unExhaustedQuotas = quotas.Where(q => q.IsExhaustedUntil is null || q.IsExhaustedUntil > SystemTime.UtcNow);
        foreach (var quota in unExhaustedQuotas)
        {
            var newUsage = await metricCalculator.CalculateUsageAsync(
                quota.Period.CalculateBegin(),
                quota.Period.CalculateEnd(),
                Address,
                cancellationToken);

            quota.UpdateExhaustion(newUsage);
        }
    }

    private IEnumerable<Quota> GetAppliedQuotasForMetricAsync(string metric, IReadOnlyCollection<Quota> quotas)
    {
        var allQuotasOfMetric = quotas.Where(q => q.MetricKey.ToString() == metric);
        var highestWeight = allQuotasOfMetric.OrderByDescending(q => q.Weight).FirstOrDefault().Weight;
        var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight).ToList();
        return appliedQuotas;
    }
}