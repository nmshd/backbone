namespace Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;
public class MetricKey
{
    public string Value { get; private set; }

    public MetricKey(string value)
    {
        Value = value;
    }
}
