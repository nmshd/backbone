using System;
using System.Text.Json;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Enmeshed.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;

public class FcmMessageBuilderTests
{
    [Fact]
    public void Message_Without_Content_Matches_Expected_Json()
    {
        // Arrange
        var notificationTitle = "someNotificationTextTitle";
        var notificationText = "someNotificationTextBody";

        // Act
        var message = new FcmMessageBuilder()
            .SetTag(1)
            .SetNotificationText(notificationTitle, notificationText)
            .Build();

        // Assert
        message.Notification.Title.Should().Be(notificationTitle);
        message.Notification.Body.Should().Be(notificationText);
        message.Android.Notification.ChannelId.Should().Be("ENMESHED");

        message.Data.Should().Contain("android_channel_id", "ENMESHED");
        
        message.Android.CollapseKey.Should().Be("1");
        message.Data.Should().Contain("tag", "1");

    }

    [Fact]
    public void Message_Data_Content_Is_Valid_Json()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        var expectedContentJson = FormatJson(@"{
          'accRef': 'id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j',
          'eventName': 'dynamic',
          'sentAt': '2021-01-01T00:00:00Z',
          'payload': {
            'SomeProperty': 'someValue'
          }
        }");

        // Act
        var message = new FcmMessageBuilder()
            .AddContent(new NotificationContent(IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), new { SomeProperty = "someValue" }))
            .Build();
        var contentJson = FormatJson(message.Data["content"]);

        // Assert
        message.Data["content-available"].Should().Be("1");
        message.Data["content"].Should().BeValidJson();
        contentJson.Should().Be(expectedContentJson);
    }

    private string FormatJson(string jsonString)
    {
        jsonString = jsonString.Replace("'", "\"");
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);
        return FormatJson(jsonElement);
    }

    private string FormatJson(object obj)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        return JsonSerializer.Serialize(obj, options);
    }
}
