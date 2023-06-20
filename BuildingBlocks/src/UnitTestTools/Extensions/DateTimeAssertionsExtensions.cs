using FluentAssertions.Primitives;
using FluentAssertions;

namespace Enmeshed.UnitTestTools.Extensions;
public static class DateTimeAssertionsExtensions
{
    public static AndConstraint<DateTimeAssertions> Be(this DateTimeAssertions it, string dateTimeString)
    {
        return it.Be(DateTime.Parse(dateTimeString));
    }

    public static AndConstraint<NullableDateTimeAssertions> Be(this NullableDateTimeAssertions it, string dateTimeString)
    {
        return it.NotBeNull().And.Be(DateTime.Parse(dateTimeString));
    }
}