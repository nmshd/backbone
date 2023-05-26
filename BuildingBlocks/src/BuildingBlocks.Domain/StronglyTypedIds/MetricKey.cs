namespace Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;
public class MetricKey
{
    public MetricKey(string value)
    {
        Value = value;
    }
    private string Value { get; }
}
