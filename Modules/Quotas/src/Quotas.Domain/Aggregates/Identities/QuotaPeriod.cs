using Enmeshed.Tooling;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public enum QuotaPeriod
{
    Hour,
    Day,
    Week,
    Month,
    Year,
    Total
}

public static class QuotaPeriodExtensions
{
    public static DateTime CalculateEnd(this QuotaPeriod period)
    {
        switch (period)
        {
            case QuotaPeriod.Hour:
                return SystemTime.UtcNow.AddHours(1);
            case QuotaPeriod.Day:
                return SystemTime.UtcNow.AddDays(1);
            case QuotaPeriod.Week:
                return SystemTime.UtcNow.AddDays(7);
            case QuotaPeriod.Month:
                return SystemTime.UtcNow.AddMonths(1);
            case QuotaPeriod.Year:
                return SystemTime.UtcNow.AddYears(1);
            case QuotaPeriod.Total:
            default:
                return SystemTime.UtcNow.AddYears(200);
        }
    }
}
