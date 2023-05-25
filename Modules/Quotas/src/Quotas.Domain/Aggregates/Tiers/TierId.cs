namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public record TierId
{
    public TierId(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static implicit operator string(TierId id)
    {
        return id.Value;
    }
}
