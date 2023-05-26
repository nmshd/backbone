using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    private readonly List<IndividualQuota> _individualQuotas = new();

    public Identity(string address, string tierId)
    {
        Address = address;
        TierId = tierId;
    }

    public string Address { get; }

    public string TierId { get; }

    public IndividualQuota CreateIndividualQuota(Metric metric, int max, QuotaPeriod period)
    {
        var individualQuota = new IndividualQuota(metric, max, period);
        _individualQuotas.Add(individualQuota);

        return individualQuota;
    }
}