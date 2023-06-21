using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.QuotaCheck;
public class CheckQuotaResult
{
    public CheckQuotaResult(IEnumerable<MetricStatus> exhaustedStatuses)
    {
        ExhaustedStatuses = exhaustedStatuses;
        IsSuccess = !exhaustedStatuses.Any();
    }

    public IEnumerable<MetricStatus> ExhaustedStatuses { get; }
    public bool IsSuccess { get; internal set; }
}
