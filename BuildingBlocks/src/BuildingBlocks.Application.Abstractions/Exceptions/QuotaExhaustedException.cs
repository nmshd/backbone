using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;

public class QuotaExhaustedException : ApplicationException
{
    public IEnumerable<ExhaustedMetricStatus> ExhaustedMetricStatuses { get;  private set; }

    public QuotaExhaustedException(MetricStatus[] exhaustedMetricStatuses) : base(GenericApplicationErrors.QuotaExhausted())
    {
        ExhaustedMetricStatuses = exhaustedMetricStatuses.Select(it=> new ExhaustedMetricStatus(it.MetricKey, it.IsExhaustedUntil));
    }
}

public class ExhaustedMetricStatus
{
    public ExhaustedMetricStatus(MetricKey metricKey, DateTime? isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil.Value;
    }

    public MetricKey MetricKey { get; }
    public DateTime IsExhaustedUntil { get; }
}