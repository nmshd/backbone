using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;

public class QuotaExhaustedException : ApplicationException
{
    public MetricKey MetricKey { get; private set; }
    public DateTime DateTime { get; private set; }

    public QuotaExhaustedException(MetricKey metricKey, DateTime dateTime) : base(GenericApplicationErrors.QuotaExhausted())
    {
        MetricKey = metricKey;
        DateTime = dateTime;
    }
}