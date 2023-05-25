using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    private List<IndividualQuota> IndividualQuotas { get; }

    public Identity(string address, string tierId)
    {
        Address = address;
        TierId = tierId;
        IndividualQuotas = new();
    }

    public string Address { get; }

    public string TierId { get; }

    public IndividualQuota CreateIndividualQuota(Metric metric, int max, QuotaPeriod period)
    {
        var individualQuota = new IndividualQuota(metric, max, period);
        IndividualQuotas.Add(individualQuota);

        return individualQuota;
    }
}