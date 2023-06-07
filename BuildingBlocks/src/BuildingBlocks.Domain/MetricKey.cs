namespace Enmeshed.BuildingBlocks.Domain;
public class MetricKey
{
    public string Value { get; private set; }

    public MetricKey(string value)
    {
        Value = value;
    }
}
