using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    private readonly List<TierQuota> _tierQuotas;

    private readonly List<MetricStatus> _metricStatuses;

    public Identity(string address, TierId tierId)
    {
        Address = address;
        TierId = tierId;
        _tierQuotas = new List<TierQuota>();
        _metricStatuses = new List<MetricStatus>();
    }

    public IReadOnlyCollection<MetricStatus> MetricStatuses => _metricStatuses.AsReadOnly();
    public IReadOnlyCollection<TierQuota> TierQuotas => _tierQuotas.AsReadOnly();
    
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // To: The dev who implements individualQuotas
    // uncomment the line below
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    internal IReadOnlyCollection<Quota> AllQuotas => _tierQuotas; //.Concat(_identityQuotas);

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
    }

    private async Task UpdateMetric(MetricKey metric, IMetricCalculator metricCalculator, CancellationToken cancellationToken)
    {
        var quotasForMetric = GetAppliedQuotasForMetric(metric);
        DateTime? globalQuotasExhaustion = null;

        foreach (var quota in quotasForMetric)
        {
            var newUsage = await metricCalculator.CalculateUsage(
                quota.Period.CalculateBegin(),
                quota.Period.CalculateEnd(),
                Address,
                cancellationToken);

            var quotaExhaustion = quota.CalculateExhaustion(newUsage);

            if(quotaExhaustion is not null && (globalQuotasExhaustion is null || quotaExhaustion > globalQuotasExhaustion))
            {
                globalQuotasExhaustion = quotaExhaustion.Value;
            }
        }

        UpdateMetricStatus(metric, globalQuotasExhaustion);
    }

    private IEnumerable<Quota> GetAppliedQuotasForMetric(MetricKey metric)
    {
        var allQuotasOfMetric = AllQuotas.Where(q => q.MetricKey == metric);
        if (!allQuotasOfMetric.Any())
        {
            return Enumerable.Empty<Quota>();
        }

        var highestWeight = allQuotasOfMetric.Max(q => q.Weight);
        var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight).ToArray();
        return appliedQuotas;
    }

    private void UpdateMetricStatus(MetricKey metricKey, DateTime? maxExhaustionDate)
    {
        var metricStatus = _metricStatuses.SingleOrDefault(m => m.MetricKey == metricKey);
        if (metricStatus != null)
        {
            metricStatus.Update(maxExhaustionDate);
        }
        else
        {
            _metricStatuses.Add(new(metricKey, Address, maxExhaustionDate));
        }
    }
}