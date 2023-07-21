using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.QuotaCheck;
public class AlwaysSuccessQuotaChecker : IQuotaChecker
{
    public Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys)
    {
        return Task.FromResult(CheckQuotaResult.Success());
    }
}
