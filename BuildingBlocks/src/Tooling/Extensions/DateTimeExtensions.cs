namespace Enmeshed.Tooling.Extensions;

public static class DateTimeExtensions
{
    private const DayOfWeek FIRST_DAY_OF_WEEK = DayOfWeek.Monday;

    public static string ToUniversalString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }

    public static DateTime StartOfHour(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime StartOfDay(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime StartOfWeek(this DateTime utcNow)
    {
        var result = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, 0, DateTimeKind.Utc);
        do result = result.AddDays(-1); while (result.DayOfWeek != FIRST_DAY_OF_WEEK);
        return result;
    }

    public static DateTime StartOfMonth(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime StartOfYear(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime EndOfHour(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 59, 59, 999, DateTimeKind.Utc);
    }

    public static DateTime EndOfDay(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 23, 59, 59, 999, DateTimeKind.Utc);
    }

    public static DateTime EndOfWeek(this DateTime utcNow)
    {
        var result = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 23, 59, 59, 999, DateTimeKind.Utc);
        while (result.DayOfWeek != FIRST_DAY_OF_WEEK) result = result.AddDays(1);
        return result.AddDays(-1);
    }

    public static DateTime EndOfMonth(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, utcNow.Month, DateTime.DaysInMonth(utcNow.Year, utcNow.Month), 23, 59, 59, 999, DateTimeKind.Utc);
    }

    public static DateTime EndOfYear(this DateTime utcNow)
    {
        return new DateTime(utcNow.Year, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
    }
}