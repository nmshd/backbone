using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
using Backbone.Tooling;
using Backbone.UnitTestTools.FluentAssertions.Extensions;

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
        message.Notification.Title.Should().Be("someNotificationTextTitle");
        message.Notification.Body.Should().Be("someNotificationTextBody");

        message.Token.Should().Contain("token1");

        message.Android.Notification.ChannelId.Should().Be("ENMESHED");
        message.Data.Should().Contain("android_channel_id", "ENMESHED");

        message.Data["content-available"].Should().Be("1");

        message.Android.CollapseKey.Should().Be("testNotificationId");
        message.Android.Notification.Tag.Should().Be("testNotificationId");
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
        actualContent.Should().BeEquivalentToJson(
            """
            {
                      'accRef': 'did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d',
                      'devicePushIdentifier' : 'DPIaaaaaaaaaaaaaaaaa',
                      'eventName': 'Test',
                      'sentAt': '2021-01-01T00:00:00.000Z',
                      'payload': {
                        'someProperty': 'someValue'
                      }
                    }
            """);
    }

    private record TestPushNotification : IPushNotification
    {
        public required string SomeProperty { get; set; }
    }
}
