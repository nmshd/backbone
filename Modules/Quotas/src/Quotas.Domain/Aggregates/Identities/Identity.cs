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
            await UpdateMetric(metric, metricCalculator, cancellationToken);
        }
        return;
    }

    private async Task UpdateMetric(string metric, IMetricCalculator metricCalculator, CancellationToken cancellationToken)
    {
        var quotas = GetAppliedQuotasForMetric(metric);
        var unexhaustedQuotas = quotas.Where(q => q.IsExhaustedUntil is null || q.IsExhaustedUntil > SystemTime.UtcNow);
        foreach (var quota in unexhaustedQuotas)
        {
            var newUsage = await metricCalculator.CalculateUsage(
                quota.Period.CalculateBegin(),
                quota.Period.CalculateEnd(),
                Address,
                cancellationToken);

            quota.UpdateExhaustion(newUsage);
        }
    }

    private IEnumerable<Quota> GetAppliedQuotasForMetric(string metric)
    {
        var allQuotasOfMetric = TierQuotas.Where(q => q.MetricKey.ToString() == metric);
        
        if (!allQuotasOfMetric.Any())
        {
            return allQuotasOfMetric;
        }

        var highestWeight = allQuotasOfMetric.Select(m => m.Weight).Max();
        var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight);
        return appliedQuotas;
    }
}