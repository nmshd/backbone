using Backbone.BuildingBlocks.Domain;

namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public class QuotaExhaustedException : ApplicationException
{
    public QuotaExhaustedException(MetricStatus[] exhaustedMetricStatuses) : base(GenericApplicationErrors.QuotaExhausted())
    {
        ExhaustedMetricStatuses = exhaustedMetricStatuses.Select(it => new ExhaustedMetricStatus(it.MetricKey, it.IsExhaustedUntil!.Value));
    }

    public IEnumerable<ExhaustedMetricStatus> ExhaustedMetricStatuses { get; }
}

public class ExhaustedMetricStatus
{
    public ExhaustedMetricStatus(MetricKey metricKey, DateTime isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public MetricKey MetricKey { get; }
    public DateTime IsExhaustedUntil { get; }
}
