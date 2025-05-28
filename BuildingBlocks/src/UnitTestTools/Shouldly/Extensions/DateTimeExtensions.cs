using Shouldly;

namespace Backbone.UnitTestTools.Shouldly.Extensions;

[ShouldlyMethods]
public static class DateTimeExtensions
{
    public static void ShouldBeAfter(this DateTime actual, DateTime expected, string? customMessage = null)
    {
        actual.AssertAwesomely(d => d > expected, actual, expected, customMessage);
    }
}
