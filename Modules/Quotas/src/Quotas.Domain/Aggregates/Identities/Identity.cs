using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Errors;
using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    private readonly List<IndividualQuota> _individualQuotas = new();
    private readonly List<TierQuota> _tierQuotas = new();

    public Identity(string address, TierId tierId)
    {
        Address = address;
        TierId = tierId;
    }

    public IReadOnlyCollection<TierQuota> TierQuotas => _tierQuotas;
    public IReadOnlyCollection<IndividualQuota> IndividualQuotas => _individualQuotas;
    public string Address { get; }
    public string TierId { get; }

    public Result<IndividualQuota, DomainError> CreateIndividualQuota(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (max <= 0)
            return Result.Failure<IndividualQuota, DomainError>(DomainErrors.MaxValueCannotBeLowerOrEqualToZero());

        var individualQuota = new IndividualQuota(metricKey, max, period, Address);
        _individualQuotas.Add(individualQuota);

        return individualQuota;
    }

    public void AssignTierQuotaFromDefinition(TierQuotaDefinition definition)
    {
        var tierQuota = new TierQuota(definition, Address);
        _tierQuotas.Add(tierQuota);
    }
}