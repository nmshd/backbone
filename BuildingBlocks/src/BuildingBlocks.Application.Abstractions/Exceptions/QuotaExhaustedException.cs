using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;

public class QuotaExhaustedException : ApplicationException
{
    public QuotaExhaustedException(ApplicationError error) : base(error)
    {
    }

    public QuotaExhaustedException(MetricKey? metricKey, DateTime dateTime) : base(GenericApplicationErrors.QuotaExhausted())
    {
        Data.Add("metricKey", metricKey);
        Data.Add("dateTime", dateTime);
    }
}