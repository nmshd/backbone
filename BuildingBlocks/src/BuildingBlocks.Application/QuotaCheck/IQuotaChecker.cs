using Backbone.BuildingBlocks.Domain;

namespace Backbone.BuildingBlocks.Application.QuotaCheck;

public interface IQuotaChecker
{
    Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys);
}
