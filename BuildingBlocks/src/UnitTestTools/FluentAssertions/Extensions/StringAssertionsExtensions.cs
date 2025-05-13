using FluentAssertions.Json;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace Backbone.UnitTestTools.FluentAssertions.Extensions;

public static class StringAssertionsExtensions
{
    public static void BeEquivalentToJson(this StringAssertions actual, string expected)
    {
        var actualContent = JToken.Parse(actual.Subject);
        var expectedContent = JToken.Parse(expected);

        actualContent.Should().BeEquivalentTo(expectedContent);
    }
}
