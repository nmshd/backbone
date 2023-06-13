using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.Tooling;

namespace Backbone.Modules.Quotas.Application.Extensions;
public static class IdentityMetricExtensions
{
    public static async Task UpdateMetrics(this Identity identity, IEnumerable<string> metrics, IMetricCalculatorFactory factory, CancellationToken cancellationToken)
    {
        foreach (var metric in metrics)
        {
            var metricCalculator = factory.CreateFor(metric);
            await identity.UpdateMetricAsync(metric, metricCalculator, cancellationToken);
        }
        return;
    }

    private static async Task UpdateMetricAsync(this Identity identity, string metric, IMetricCalculator metricCalculator, CancellationToken cancellationToken)
    {
        var quotas = GetAppliedQuotasForMetricAsync(metric, identity.TierQuotas);
        var unExhaustedQuotas = quotas.Where(q => q.IsExhaustedUntil is null || q.IsExhaustedUntil > SystemTime.UtcNow);
        foreach (var quota in unExhaustedQuotas)
        {
            var newUsage = await metricCalculator.CalculateUsageAsync(
                quota.Period.CalculateBegin(),
                quota.Period.CalculateEnd(),
                identity.Address,
                cancellationToken);

            quota.UpdateExhaustion(newUsage);
        }
    }

    private static IEnumerable<Quota> GetAppliedQuotasForMetricAsync(string metric, IReadOnlyCollection<Quota> quotas)
    {
        var allQuotasOfMetric = quotas.Where(q=>q.MetricKey.ToString() == metric);
        var highestWeight = allQuotasOfMetric.OrderByDescending(q => q.Weight).FirstOrDefault().Weight;
        var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight).ToList();
        return appliedQuotas;
    }
}
