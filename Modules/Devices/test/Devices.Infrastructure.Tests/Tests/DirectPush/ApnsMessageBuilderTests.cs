using System.Text.Json;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;
public class ApnsMessageBuilderTests
{
    [Fact]
    public void Built_message_has_all_properties_set()
    {
        // Act
        var request = new ApnsMessageBuilder("someAppBundleIdentifier", "https://api.development.push.apple.com/3/device/someDeviceId", "someValidJwt").Build();

        // Assert
        request.RequestUri!.ToString().Should().Contain("https://api.development.push.apple.com/3/device/someDeviceId");
        request.Headers.GetValues("apns-topic").FirstOrDefault().Should().Be("someAppBundleIdentifier");
        request.Headers.GetValues("apns-expiration").FirstOrDefault().Should().Be("0");
        request.Headers.GetValues("apns-push-type").FirstOrDefault().Should().Be("alert");
        request.Headers.GetValues("apns-priority").FirstOrDefault().Should().Be("5");
        request.Headers.GetValues("Authorization").FirstOrDefault().Should().NotBeNull();
    }

    [Fact]
    public async Task Content_is_valid_json()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        // Act
        var request = new ApnsMessageBuilder("someAppBundleIdentifier", "https://api.development.push.apple.com/3/device/someDeviceId", "someValidJwt")
            .AddContent(new NotificationContent(IdentityAddress.Parse("did:web:prod.enmesh.eu:dids:MTkcyw1T29xwRqHjSsAMrY4HvjHFALPfJ"), DevicePushIdentifier.Parse("DPIaaaaaaaaaaaaaaaaa"), new { SomeProperty = "someValue" }))
            .SetNotificationText("someNotificationTextTitle", "someNotificationTextBody")
            .SetNotificationId(1)
            .Build();
        var requestBody = FormatJson(await request.Content!.ReadAsStringAsync());

        // Assert
        requestBody.Should().Be(FormatJson(@"{
            'notId': 1,
            'content': {
                'accRef': 'did:web:prod.enmesh.eu:dids:MTkcyw1T29xwRqHjSsAMrY4HvjHFALPfJ',
                'devicePushIdentifier' : 'DPIaaaaaaaaaaaaaaaaa',
                'eventName': 'dynamic',
                'sentAt': '2021-01-01T00:00:00.000Z',
                'payload': {
                    'someProperty': 'someValue'
                }
            },
            'aps': {
                'content-available': '1',
                'alert': {
                    'title': 'someNotificationTextTitle',
                    'body': 'someNotificationTextBody'
                }
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
