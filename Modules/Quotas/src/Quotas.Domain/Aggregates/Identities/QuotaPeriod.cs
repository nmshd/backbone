using Backbone.Tooling;
using Backbone.Tooling.Extensions;

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
    public static DateTime CalculateBegin(this QuotaPeriod period)
    {
        var utcNow = SystemTime.UtcNow;

        switch (period)
        {
            case QuotaPeriod.Hour:
                return utcNow.StartOfHour();
            case QuotaPeriod.Day:
                return utcNow.StartOfDay();
            case QuotaPeriod.Week:
                return utcNow.StartOfWeek();
            case QuotaPeriod.Month:
                return utcNow.StartOfMonth();
            case QuotaPeriod.Year:
                return utcNow.StartOfYear();
            case QuotaPeriod.Total:
                return DateTime.MinValue;
            default:
                throw new NotImplementedException($"Cannot calculate begin of the passed period '{period}'.");
        }
    }

    public static DateTime CalculateEnd(this QuotaPeriod period)
    {
        var utcNow = SystemTime.UtcNow;

        switch (period)
        {
            case QuotaPeriod.Hour:
                return utcNow.EndOfHour();
            case QuotaPeriod.Day:
                return utcNow.EndOfDay();
            case QuotaPeriod.Week:
                return utcNow.EndOfWeek();
            case QuotaPeriod.Month:
                return utcNow.EndOfMonth();
            case QuotaPeriod.Year:
                return utcNow.EndOfYear();
            case QuotaPeriod.Total:
                return DateTime.MaxValue;
            default:
                throw new NotImplementedException($"Cannot calculate end of the passed period '{period}'.");
        }
    }
}
