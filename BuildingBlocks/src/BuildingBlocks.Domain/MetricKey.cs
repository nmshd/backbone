namespace Backbone.BuildingBlocks.Domain;

public record MetricKey(string Value)
{
    public string Value { get; } = Value;

    public static implicit operator string(MetricKey id)
    {
        return id.Value;
    }
}
