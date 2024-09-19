using Backbone.BuildingBlocks.Domain;

namespace Backbone.BuildingBlocks.Application.QuotaCheck;

public class AlwaysSuccessQuotaChecker : IQuotaChecker
{
    public Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys)
    {
        return Task.FromResult(CheckQuotaResult.Success());
    }
}
