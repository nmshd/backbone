using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Tooling;

namespace Backbone.Modules.Quotas.Application.Extensions;
public static class IdentityMetricExtensions
{
    public static async Task UpdateMetrics(this Identity identity, IEnumerable<string> metrics, IMetricCalculatorFactory factory, IQuotasRepository quotasRepository, CancellationToken cancellationToken)
    {
        foreach (var metric in metrics)
        {
            var metricCalculator = factory.CreateFor(metric);
            await identity.UpdateMetricAsync(metric, metricCalculator, quotasRepository, cancellationToken);
        }
        return;
    }

    private static async Task UpdateMetricAsync(this Identity identity, string metric, IMetricCalculator metricCalculator, IQuotasRepository quotasRepository, CancellationToken cancellationToken)
    {
        var quotas = await GetAppliedQuotasForMetricAsync(metric, quotasRepository, cancellationToken);
        var unExhaustedQuotas = quotas.Where(q => q.IsExhaustedUntil is null || q.IsExhaustedUntil > SystemTime.UtcNow);
        foreach (var quota in unExhaustedQuotas)
        {
            var newValue = metricCalculator.CalculateUsage(
                SystemTime.UtcNow.AddDays(-30),
                SystemTime.UtcNow,
                identity.Address);

            quota.UpdateExhaustion(newValue);
        }
    }

    private static async Task<IEnumerable<Quota>> GetAppliedQuotasForMetricAsync(string metric, IQuotasRepository quotasRepository, CancellationToken cancellationToken)
    {
        var allQuotasOfMetric = await quotasRepository.FindQuotasByMetricKey(metric, cancellationToken);
        var highestWeight = allQuotasOfMetric.OrderByDescending(q => q.Weight).FirstOrDefault().Weight;
        var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight && q.IsCurrentlyValid()).ToList();
        return appliedQuotas;
    }
}
