namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public record TierId(string Value)
{
    public static implicit operator string(TierId id)
    {
        return id.Value;
    }
}
