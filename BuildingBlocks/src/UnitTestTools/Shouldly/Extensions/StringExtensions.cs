using System.Text.Json;
using Shouldly;

namespace Backbone.UnitTestTools.Shouldly.Extensions;

[ShouldlyMethods]
public static class StringExtensions
{
    public static void ShouldBeEquivalentToJson(this string actual, string expected, string? customMessage = null)
    {
        var actualDoc = JsonDocument.Parse(actual);
        var expectedDoc = JsonDocument.Parse(expected);

        actualDoc.AssertAwesomely(v => JsonElement.DeepEquals(v.RootElement, expectedDoc.RootElement), actual, expected, customMessage);
    }
}
