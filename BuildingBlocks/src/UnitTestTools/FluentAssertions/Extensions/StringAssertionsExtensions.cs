using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Enmeshed.UnitTestTools.FluentAssertions.Extensions;
public static class StringAssertionsExtensions
{
    public static AndConstraint<DateTimeAssertions> Be(this DateTimeAssertions it, string dateTimeString)
    {
        return it.Be(DateTime.Parse(dateTimeString));
    }

    public static AndConstraint<StringAssertions> BeValidJson(this StringAssertions it)
    {
        try
        {
            JsonDocument.Parse(it.Subject);
            Execute.Assertion.Given(() => it);
        }
        catch (Exception _)
        {
            Execute.Assertion.FailWith("Invalid Json");
        }

        return new AndConstraint<StringAssertions>(it);
    }
}
