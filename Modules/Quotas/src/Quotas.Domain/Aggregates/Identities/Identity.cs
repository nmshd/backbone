using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    private readonly List<TierQuota> _tierQuotas = new();

    public Identity(string address, string tierId)
    {
        Address = address;
        TierId = tierId;
    }

    public string Address { get; }
    public string TierId { get; }

    public void AssignTierQuotaFromDefinition(TierQuotaDefinition definition)
    {
        var tierQuota = new TierQuota(definition, Address);
        _tierQuotas.Add(tierQuota);
    }
}