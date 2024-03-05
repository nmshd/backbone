using Backbone.BuildingBlocks.Domain;

namespace Backbone.BuildingBlocks.Application.QuotaCheck;
public class CheckQuotaResult
{
    public CheckQuotaResult(IEnumerable<MetricStatus> exhaustedStatuses)
    {
        ExhaustedStatuses = exhaustedStatuses;
        IsSuccess = !exhaustedStatuses.Any();
    }

    public IEnumerable<MetricStatus> ExhaustedStatuses { get; }
    public bool IsSuccess { get; internal set; }

    public static CheckQuotaResult Success() => new(Enumerable.Empty<MetricStatus>());
}
