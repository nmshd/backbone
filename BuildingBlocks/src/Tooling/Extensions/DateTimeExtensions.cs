namespace Backbone.Tooling.Extensions;

public static class DateTimeExtensions
{
    private const DayOfWeek FIRST_DAY_OF_WEEK = DayOfWeek.Monday;

    public static string ToUniversalString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }

    public static DateTime StartOfHour(this DateTime pivot)
    {
        return new DateTime(pivot.Year, pivot.Month, pivot.Day, pivot.Hour, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime StartOfDay(this DateTime pivot)
    {
        return new DateTime(pivot.Year, pivot.Month, pivot.Day, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime StartOfWeek(this DateTime pivot)
    {
        var result = new DateTime(pivot.Year, pivot.Month, pivot.Day, 0, 0, 0, 0, DateTimeKind.Utc);
        do result = result.AddDays(-1); while (result.DayOfWeek != FIRST_DAY_OF_WEEK);
        return result;
    }

    public static DateTime StartOfMonth(this DateTime pivot)
    {
        return new DateTime(pivot.Year, pivot.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime StartOfYear(this DateTime pivot)
    {
        return new DateTime(pivot.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime EndOfHour(this DateTime pivot)
    {
        return new DateTime(pivot.Year, pivot.Month, pivot.Day, pivot.Hour, 59, 59, 999, DateTimeKind.Utc);
    }

    public static DateTime EndOfDay(this DateTime pivot)
    {
        return new DateTime(pivot.Year, pivot.Month, pivot.Day, 23, 59, 59, 999, DateTimeKind.Utc);
    }

    public static DateTime EndOfWeek(this DateTime pivot)
    {
        var result = new DateTime(pivot.Year, pivot.Month, pivot.Day, 23, 59, 59, 999, DateTimeKind.Utc);
        while (result.DayOfWeek != FIRST_DAY_OF_WEEK) result = result.AddDays(1);
        return result.AddDays(-1);
    }

    public static DateTime EndOfMonth(this DateTime pivot)
    {
        return new DateTime(pivot.Year, pivot.Month, DateTime.DaysInMonth(pivot.Year, pivot.Month), 23, 59, 59, 999, DateTimeKind.Utc);
    }

    public static DateTime EndOfYear(this DateTime pivot)
    {
        return new DateTime(pivot.Year, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
    }
}
