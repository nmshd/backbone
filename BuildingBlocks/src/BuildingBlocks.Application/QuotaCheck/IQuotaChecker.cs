using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.QuotaCheck;
public interface IQuotaChecker
{
    Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys);
}
