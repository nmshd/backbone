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
    public static DateTime CalculateBegin(this QuotaPeriod period)
    {
        var utcNow = SystemTime.UtcNow;

        switch (period)
        {
            case QuotaPeriod.Hour:
                return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 0, 0, 0, DateTimeKind.Utc);
            case QuotaPeriod.Day:
                return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, 0, DateTimeKind.Utc);
            case QuotaPeriod.Week:
                var result = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, 0, DateTimeKind.Utc);
                do result = result.AddDays(-1); while (result.DayOfWeek != DayOfWeek.Sunday);
                return result;
            case QuotaPeriod.Month:
                return new DateTime(utcNow.Year, utcNow.Month, DateTime.DaysInMonth(utcNow.Year, utcNow.Month), 23, 59, 59, 999, DateTimeKind.Utc);
            case QuotaPeriod.Year:
                return new DateTime(utcNow.Year, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            case QuotaPeriod.Total:
            default:
                return SystemTime.UtcNow.AddYears(200);
        }
    }

    public static DateTime CalculateEnd(this QuotaPeriod period)
    {
        var utcNow = SystemTime.UtcNow;

        switch (period)
        {
            case QuotaPeriod.Hour:
                return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 59, 59, 999, DateTimeKind.Utc);
            case QuotaPeriod.Day:
                return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 23, 59, 59, 999, DateTimeKind.Utc);
            case QuotaPeriod.Week:
                var result = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 23, 59, 59, 999, DateTimeKind.Utc);
                while (result.DayOfWeek != DayOfWeek.Sunday) result = result.AddDays(1);
                return result.AddDays(-1);
            case QuotaPeriod.Month:
                return new DateTime(utcNow.Year, utcNow.Month, DateTime.DaysInMonth(utcNow.Year, utcNow.Month), 23, 59, 59, 999, DateTimeKind.Utc);
            case QuotaPeriod.Year:
                return new DateTime(utcNow.Year, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            case QuotaPeriod.Total:
            default:
                return SystemTime.UtcNow.AddYears(200);
        }
    }
}
