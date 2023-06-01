using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;

public class QuotaExhaustedException : ApplicationException
{
    public QuotaExhaustedException(MetricKey metricKey, DateTime isExhaustedUntil) : base(GenericApplicationErrors.QuotaExhausted())
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public MetricKey MetricKey { get; }
    public DateTime IsExhaustedUntil { get; }
}