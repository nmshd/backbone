using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.Connectors.Fcm;

public class FcmMessageBuilderTests : AbstractTestsBase
{
    [Fact]
    public void Built_message_has_all_properties_set()
    {
        // Act
        var message = new FcmMessageBuilder()
            .SetTag("testNotificationId")
            .SetToken("token1")
            .SetNotificationText("someNotificationTextTitle", "someNotificationTextBody")
            .AddContent(new NotificationContent(IdentityAddress.Parse("did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d"), DevicePushIdentifier.New(),
                new TestPushNotification { SomeProperty = "someValue" }))
            .Build();

        // Assert
        message.Notification.Title.ShouldBe("someNotificationTextTitle");
        message.Notification.Body.ShouldBe("someNotificationTextBody");

        message.Token.ShouldContain("token1");

        message.Android.Notification.ChannelId.ShouldBe("ENMESHED");
        message.Data.ShouldContain(new KeyValuePair<string, string>("android_channel_id", "ENMESHED"));

        message.Data["content-available"].ShouldBe("1");

        message.Android.CollapseKey.ShouldBe("testNotificationId");
        message.Android.Notification.Tag.ShouldBe("testNotificationId");
    }

    [Fact]
    public void Content_is_valid_json()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        // Act
        var message = new FcmMessageBuilder()
            .AddContent(new NotificationContent(IdentityAddress.Parse("did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d"), DevicePushIdentifier.Parse("DPIaaaaaaaaaaaaaaaaa"),
                new TestPushNotification { SomeProperty = "someValue" }))
            .Build();
        var actualContent = message.Data["content"];

        // Assert
        actualContent.ShouldBeEquivalentToJson(
            """
            {
                      "accRef": "did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d",
                      "devicePushIdentifier" : "DPIaaaaaaaaaaaaaaaaa",
                      "eventName": "Test",
                      "sentAt": "2021-01-01T00:00:00.000Z",
                      "payload": {
                        "someProperty": "someValue"
                      }
                    }
            """);
    }

    private record TestPushNotification : IPushNotification
    {
        public required string SomeProperty { get; set; }
    }
}
