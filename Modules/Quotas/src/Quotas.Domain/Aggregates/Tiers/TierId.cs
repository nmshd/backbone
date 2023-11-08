namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public record TierId(string Value)
{
    public const string UP_FOR_DELETION_DEFAULT_ID = "TIR00000000000000001";

    public static implicit operator string(TierId id)
    {
        return id.Value;
    }
}
