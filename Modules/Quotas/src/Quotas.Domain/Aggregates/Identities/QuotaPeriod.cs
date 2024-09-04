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
    public static DateTime CalculateBegin(this QuotaPeriod period, DateTime utcNow)
    {
        return period switch
        {
            QuotaPeriod.Hour => utcNow.StartOfHour(),
            QuotaPeriod.Day => utcNow.StartOfDay(),
            QuotaPeriod.Week => utcNow.StartOfWeek(),
            QuotaPeriod.Month => utcNow.StartOfMonth(),
            QuotaPeriod.Year => utcNow.StartOfYear(),
            QuotaPeriod.Total => DateTime.MinValue,
            _ => throw new NotSupportedException($"Cannot calculate begin of the passed period '{period}'.")
        };
    }

    public static DateTime CalculateEnd(this QuotaPeriod period, DateTime utcNow)
    {
        return period switch
        {
            QuotaPeriod.Hour => utcNow.EndOfHour(),
            QuotaPeriod.Day => utcNow.EndOfDay(),
            QuotaPeriod.Week => utcNow.EndOfWeek(),
            QuotaPeriod.Month => utcNow.EndOfMonth(),
            QuotaPeriod.Year => utcNow.EndOfYear(),
            QuotaPeriod.Total => DateTime.MaxValue,
            _ => throw new NotSupportedException($"Cannot calculate end of the passed period '{period}'.")
        };
    }
}
