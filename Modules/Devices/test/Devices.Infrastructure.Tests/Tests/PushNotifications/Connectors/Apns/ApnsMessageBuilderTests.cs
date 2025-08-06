using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.Connectors.Apns;

public class ApnsMessageBuilderTests : AbstractTestsBase
{
    [Fact]
    public void Built_message_has_all_properties_set()
    {
        // Act
        var request = new ApnsMessageBuilder("someAppBundleIdentifier", PushEnvironment.Development, ApnsHandle.Parse("someDeviceId").Value, "someValidJwt")
            .SetNotificationId("testNotificationId")
            .Build();

        // Assert
        request.RequestUri!.ToString().ShouldContain("https://api.sandbox.push.apple.com/3/device/someDeviceId");
        request.Headers.GetValues("apns-topic").FirstOrDefault().ShouldBe("someAppBundleIdentifier");
        request.Headers.GetValues("apns-expiration").FirstOrDefault().ShouldBe("0");
        request.Headers.GetValues("apns-push-type").FirstOrDefault().ShouldBe("alert");
        request.Headers.GetValues("apns-priority").FirstOrDefault().ShouldBe("5");
        request.Headers.GetValues("Authorization").FirstOrDefault().ShouldNotBeNull();
        request.Headers.GetValues("apns-collapse-id").FirstOrDefault().ShouldBe("testNotificationId");
    }

    [Fact]
    public async Task Content_is_valid_json()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        // Act
        var request = new ApnsMessageBuilder("someAppBundleIdentifier", PushEnvironment.Development, ApnsHandle.Parse("someDeviceId").Value, "someValidJwt")
            .AddContent(new NotificationContent(IdentityAddress.Parse("did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d"), DevicePushIdentifier.Parse("DPIaaaaaaaaaaaaaaaaa"),
                new TestPushNotification { SomeProperty = "someValue" }))
            .SetNotificationText("someNotificationTextTitle", "someNotificationTextBody")
            .SetNotificationId("testNotificationId")
            .Build();
        var actualContent = await request.Content!.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        actualContent.ShouldBeEquivalentToJson(
            """
            {
                "content": {
                    "accRef": "did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d",
                    "devicePushIdentifier" : "DPIaaaaaaaaaaaaaaaaa",
                    "eventName": "Test",
                    "sentAt": "2021-01-01T00:00:00.000Z",
                    "payload": {
                        "someProperty": "someValue"
                    }
                },
                "aps": {
                    "content-available": "1",
                    "alert": {
                        "title": "someNotificationTextTitle",
                        "body": "someNotificationTextBody"
                    }
                }
            }
            """
        );
    }

    private record TestPushNotification : IPushNotification
    {
        public required string SomeProperty { get; set; }
    }
}
