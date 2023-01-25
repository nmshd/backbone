using System;
using System.Text.Json;
using Devices.Infrastructure.PushNotifications;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Xunit;

namespace Devices.Infrastructure.Tests.Tests
{
    public class FcmNotificationBuilderTests
    {
        [Fact]
        public void Test1()
        {
            SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

            var builtNotification = NotificationBuilder
                .Create(NotificationPlatform.Fcm)
                .SetTag(1)
                .SetNotificationText("someNotificationTextTitle", "someNotificationTextBody")
                .AddContent(new NotificationContent(IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), new {SomeProperty = "someValue"}))
                .Build();

            var formattedBuiltNotification = FormatJson(builtNotification.Body);

            var expectedNotification = FormatJson(@"{
                'data': {
                    'android_channel_id': 'ENMESHED',
                    'content-available': '1',
                    'content': {
                        'accRef': 'id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j',
                        'eventName': 'dynamic',
                        'sentAt': '2021-01-01T00:00:00.000Z',
                        'payload': {
                            'someProperty': 'someValue'
                        }
                    }
                },
                'notification': {
                    'tag': '1',
                    'title': 'someNotificationTextTitle',
                    'body': 'someNotificationTextBody'
                }
            }");

            formattedBuiltNotification.Should().Be(expectedNotification);

            builtNotification.Headers.Should().BeEmpty();
            builtNotification.ContentType.Should().Be("application/json;charset=utf-8");
        }

        private string FormatJson(string jsonString)
        {
            jsonString = jsonString.Replace("'", "\"");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);
            return JsonSerializer.Serialize(jsonElement, options);
        }
    }
}
