using System;
using System.Text.Json;
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

        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        var notificationTitle = "someNotificationTextTitle";
        var notificationText = "someNotificationTextBody";
        var expectedNotification = FormatJson(@"{
          'Tokens': null,
          'Data': {
            'android_channel_id': 'ENMESHED',
            'tag': '1'
          },
          'Notification': {
            'Title': 'someNotificationTextTitle',
            'Body': 'someNotificationTextBody',
            'ImageUrl': null
          },
          'Android': {
            'CollapseKey': '1',
            'Priority': null,
            'TimeToLive': null,
            'RestrictedPackageName': null,
            'Data': null,
            'Notification': {
              'Title': null,
              'Body': null,
              'Icon': null,
              'Color': null,
              'Sound': null,
              'Tag': null,
              'ImageUrl': null,
              'ClickAction': null,
              'TitleLocKey': null,
              'TitleLocArgs': null,
              'BodyLocKey': null,
              'BodyLocArgs': null,
              'ChannelId': 'ENMESHED',
              'Ticker': null,
              'Sticky': false,
              'EventTimestamp': '0001-01-01T00:00:00',
              'LocalOnly': false,
              'Priority': null,
              'VibrateTimingsMillis': null,
              'DefaultVibrateTimings': false,
              'DefaultSound': false,
              'LightSettings': null,
              'DefaultLightSettings': false,
              'Visibility': null,
              'NotificationCount': null
            },
            'FcmOptions': null
          },
          'Webpush': null,
          'Apns': null
        }");

        // Act
        var message = new FcmMessageBuilder()
            .SetTag(1)
            .SetNotificationText(notificationTitle, notificationText)
            .Build();
        var messageJson = FormatJson(message);

        // Assert
        messageJson.Should().Be(expectedNotification);
    }

    [Fact]
    public void Message_Data_Content_Is_Valid_Json()
    {
        // Arrange

        // Act
        var message = new FcmMessageBuilder()
            .AddContent(new NotificationContent(IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), new { SomeProperty = "someValue" }))
            .Build();

        // Assert
        message.Data["content"].Should().BeValidJson();
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
