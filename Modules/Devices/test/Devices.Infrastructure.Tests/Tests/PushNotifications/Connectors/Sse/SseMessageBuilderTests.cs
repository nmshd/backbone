using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using Backbone.UnitTestTools.Shouldly.Extensions;

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
        request.Method.ShouldBe(HttpMethod.Post);
        request.RequestUri?.ToString().ShouldBe("recipient-address/events");

        request.Content.ShouldNotBeNull();
        var actualContent = await request.Content!.ReadAsStringAsync(TestContext.Current.CancellationToken);
        actualContent.ShouldBeEquivalentToJson("""{ "eventName": "Test" }""");
    }
}
