using FluentAssertions;
using FluentAssertions.Primitives;

namespace Backbone.UnitTestTools.Extensions;

public static class DateTimeAssertionsExtensions
{
    public static AndConstraint<DateTimeAssertions> Be(this DateTimeAssertions it, string dateTimeString)
    {
        return it.Be(DateTime.Parse(dateTimeString));
    }

    public static AndConstraint<NullableDateTimeAssertions> BeEndOfHour(this NullableDateTimeAssertions it)
    {
        return it.NotBeNull()
            .And.HaveMinute(59)
            .And.HaveSecond(59);
    }

    public static AndConstraint<NullableDateTimeAssertions> BeEndOfDay(this NullableDateTimeAssertions it)
    {
        return it.NotBeNull()
            .And.HaveHour(23)
            .And.HaveMinute(59)
            .And.HaveSecond(59);
    }

    public static AndConstraint<NullableDateTimeAssertions> Be(this NullableDateTimeAssertions it,
        string dateTimeString)
    {
        return it.NotBeNull().And.Be(DateTime.Parse(dateTimeString));
    }
}
