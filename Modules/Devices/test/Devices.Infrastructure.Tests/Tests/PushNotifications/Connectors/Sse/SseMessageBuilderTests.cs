using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.Connectors.Sse;

public class SseMessageBuilderTests : AbstractTestsBase
{
    [Fact]
    public async Task Builds_the_expected_http_request()
    {
        // Arrange
        var sseMessageBuilder = new SseMessageBuilder("recipient-address", "Test");

        // Act
        var request = sseMessageBuilder.Build();

        // Assert
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri.Should().Be("recipient-address/events");

        request.Content.Should().NotBeNull();
        var actualContent = await request.Content!.ReadAsStringAsync();
        actualContent.Should().BeEquivalentToJson("""{ "eventName": "Test" }""");
    }
}
