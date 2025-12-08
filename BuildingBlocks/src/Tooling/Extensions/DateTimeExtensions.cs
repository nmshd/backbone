namespace Backbone.Tooling.Extensions;

public static class DateTimeExtensions
{
    private const DayOfWeek FIRST_DAY_OF_WEEK = DayOfWeek.Monday;

    extension(DateTime dateTime)
    {
        public string ToUniversalString()
        {
            return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }

        public DateTime StartOfHour()
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0, DateTimeKind.Utc);
        }

        public DateTime StartOfDay()
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public DateTime StartOfWeek()
        {
            var result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, DateTimeKind.Utc);

            while (result.DayOfWeek != FIRST_DAY_OF_WEEK) result = result.AddDays(-1);

            return result;
        }

        public DateTime StartOfMonth()
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public DateTime StartOfYear()
        {
            return new DateTime(dateTime.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public DateTime EndOfHour()
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 59, 59, 999, DateTimeKind.Utc);
        }

        public DateTime EndOfDay()
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, DateTimeKind.Utc);
        }

        public DateTime EndOfWeek()
        {
            var result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, DateTimeKind.Utc);
            while (result.DayOfWeek != FIRST_DAY_OF_WEEK) result = result.AddDays(1);
            return result.AddDays(-1);
        }

        public DateTime EndOfMonth()
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month), 23, 59, 59, 999, DateTimeKind.Utc);
        }

        public DateTime EndOfYear()
        {
            return new DateTime(dateTime.Year, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        }

        public int DaysUntilDate()
        {
            return (dateTime - SystemTime.UtcNow).Days;
        }
    }
}
