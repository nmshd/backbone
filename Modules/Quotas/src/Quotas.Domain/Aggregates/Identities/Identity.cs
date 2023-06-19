using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    // To: The dev who implements individualQuotas
    // Ensure that all palces where *all* quotas are to be used reference
    // both _tierQuotas and _individualQuotas (to be created).
    // e.g. the UpdateMetric method
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

    public async Task UpdateMetrics(IEnumerable<MetricKey> metrics, MetricCalculatorFactory factory, CancellationToken cancellationToken)
    {
        foreach (var metric in metrics)
        {
            var metricCalculator = factory.CreateFor(metric);
            await UpdateMetric(metric, metricCalculator, cancellationToken);
        }
        return;
    }

    private async Task UpdateMetric(MetricKey metric, IMetricCalculator metricCalculator, CancellationToken cancellationToken)
    {
        var quotas = GetAppliedQuotasForMetric(metric, TierQuotas);
        // https://github.com/nmshd/backbone/pull/160/files/eb01b70cb351054d62e769f28fbee11c1cc8e080..c33630ef83807c85927091c17cefe27bd52083e2#r1230794032
        foreach (var quota in quotas)
        {
            var newUsage = await metricCalculator.CalculateUsage(
                quota.Period.CalculateBegin(),
                quota.Period.CalculateEnd(),
                Address,
                cancellationToken);

            quota.UpdateExhaustion(newUsage);
        }
    }

    private IEnumerable<Quota> GetAppliedQuotasForMetric(MetricKey metric, IEnumerable<Quota> quotas)
    {
        try
        {
            var allQuotasOfMetric = quotas.Where(q => q.MetricKey == metric);
            var highestWeight = allQuotasOfMetric.OrderByDescending(q => q.Weight).Max(q=>q.Weight);
            var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight).ToList();
            return appliedQuotas;
        }
        catch (InvalidOperationException _)
        {
            return Enumerable.Empty<Quota>();
        }
    }
}