using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;

namespace Enmeshed.BuildingBlocks.Application.Attributes;
public class ApplyQuotasForMetricsAttribute : Attribute
{
    private MetricKey[] Attributes { get; }

    public ApplyQuotasForMetricsAttribute(params string[] attributes)
    {
        Attributes = attributes.Select(it=> new MetricKey(it)).ToArray();
    }
}
