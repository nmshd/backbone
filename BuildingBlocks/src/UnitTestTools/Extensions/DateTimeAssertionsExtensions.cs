using FluentAssertions.Primitives;
using FluentAssertions;
using Enmeshed.Tooling;

namespace Enmeshed.UnitTestTools.Extensions;
public static class DateTimeAssertionsExtensions
{
    public static AndConstraint<DateTimeAssertions> Be(this DateTimeAssertions it, string dateTimeString)
    {
        return it.Be(DateTime.Parse(dateTimeString));
    }

    public static AndConstraint<NullableDateTimeAssertions> BeEndOfHour(this NullableDateTimeAssertions it)
    {
        var utcNow = SystemTime.UtcNow;
        return it.NotBeNull().
            And.HaveYear(utcNow.Year).
            And.HaveMonth(utcNow.Month).
            And.HaveDay(utcNow.Day).
            And.HaveHour(utcNow.Hour).
            And.HaveMinute(59).
            And.HaveSecond(59);
    }

    public static AndConstraint<NullableDateTimeAssertions> BeEndOfDay(this NullableDateTimeAssertions it)
    {
        var utcNow = SystemTime.UtcNow;
        return it.NotBeNull().
            And.HaveYear(utcNow.Year).
            And.HaveMonth(utcNow.Month).
            And.HaveDay(utcNow.Day).
            And.HaveHour(23).
            And.HaveMinute(59).
            And.HaveSecond(59);
    }

    public static AndConstraint<NullableDateTimeAssertions> Be(this NullableDateTimeAssertions it, string dateTimeString)
    {
        return it.NotBeNull().And.Be(DateTime.Parse(dateTimeString));
    }
}