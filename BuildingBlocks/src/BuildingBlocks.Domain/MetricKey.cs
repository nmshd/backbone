namespace Enmeshed.BuildingBlocks.Domain;
public class MetricKey
{
    public string Value { get; private set; }

    public MetricKey(string value)
    {
        Value = value;
    }

    public static implicit operator string(MetricKey id)
    {
        return id.Value;
    }
}
