using System.Text.Json;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;

public class FcmMessageBuilderTests
{
    [Fact]
    public void Built_message_has_all_properties_set()
    {
        // Act
        var message = new FcmMessageBuilder()
            .SetTag(1)
            .SetToken("token1")
            .SetNotificationText("someNotificationTextTitle", "someNotificationTextBody")
            .AddContent(new NotificationContent(IdentityAddress.Parse("did:e:prod.enmeshed.eu:dids:b9d25bd0a2bbd3aa48437c"), DevicePushIdentifier.New(), new { SomeProperty = "someValue" }))
            .Build();

        // Assert
        message.Notification.Title.Should().Be("someNotificationTextTitle");
        message.Notification.Body.Should().Be("someNotificationTextBody");

        message.Token.Should().Contain("token1");

        message.Android.Notification.ChannelId.Should().Be("ENMESHED");
        message.Data.Should().Contain("android_channel_id", "ENMESHED");

        message.Data["content-available"].Should().Be("1");

        message.Android.CollapseKey.Should().Be("1");
        message.Data.Should().Contain("tag", "1");
    }

    [Fact]
    public void Content_is_valid_json()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        // Act
        var message = new FcmMessageBuilder()
            .AddContent(new NotificationContent(IdentityAddress.Parse("did:e:prod.enmeshed.eu:dids:b9d25bd0a2bbd3aa48437c"), DevicePushIdentifier.Parse("DPIaaaaaaaaaaaaaaaaa"), new { SomeProperty = "someValue" }))
            .Build();
        var contentJson = FormatJson(message.Data["content"]);

        // Assert
        contentJson.Should().Be(FormatJson(@"{
          'accRef': 'did:e:prod.enmeshed.eu:dids:b9d25bd0a2bbd3aa48437c',
          'devicePushIdentifier' : 'DPIaaaaaaaaaaaaaaaaa',
          'eventName': 'dynamic',
          'sentAt': '2021-01-01T00:00:00.000Z',
          'payload': {
            'someProperty': 'someValue'
          }
        }"));
    }

    private static string FormatJson(string jsonString)
    {
        jsonString = jsonString.Replace("'", "\"");

        var deserialized = JsonSerializer.Deserialize<JsonElement>(jsonString);

        return JsonSerializer.Serialize(deserialized, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
    }
}
