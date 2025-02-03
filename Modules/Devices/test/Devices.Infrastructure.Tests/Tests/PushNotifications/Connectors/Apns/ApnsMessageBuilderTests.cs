using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Tooling;
using Backbone.UnitTestTools.FluentAssertions.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.Connectors.Apns;

public class ApnsMessageBuilderTests : AbstractTestsBase
{
    [Fact]
    public void Built_message_has_all_properties_set()
    {
        // Act
        var request = new ApnsMessageBuilder("someAppBundleIdentifier", "https://api.development.push.apple.com/3/device/someDeviceId", "someValidJwt").SetNotificationId("testNotificationId").Build();

        // Assert
        request.RequestUri!.ToString().Should().Contain("https://api.development.push.apple.com/3/device/someDeviceId");
        request.Headers.GetValues("apns-topic").FirstOrDefault().Should().Be("someAppBundleIdentifier");
        request.Headers.GetValues("apns-expiration").FirstOrDefault().Should().Be("0");
        request.Headers.GetValues("apns-push-type").FirstOrDefault().Should().Be("alert");
        request.Headers.GetValues("apns-priority").FirstOrDefault().Should().Be("5");
        request.Headers.GetValues("Authorization").FirstOrDefault().Should().NotBeNull();
        request.Headers.GetValues("apns-collapse-id").FirstOrDefault().Should().Be("testNotificationId");
    }

    [Fact]
    public async Task Content_is_valid_json()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        // Act
        var request = new ApnsMessageBuilder("someAppBundleIdentifier", "https://api.development.push.apple.com/3/device/someDeviceId", "someValidJwt")
            .AddContent(new NotificationContent(IdentityAddress.Parse("did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d"), DevicePushIdentifier.Parse("DPIaaaaaaaaaaaaaaaaa"),
                new TestPushNotification { SomeProperty = "someValue" }))
            .SetNotificationText("someNotificationTextTitle", "someNotificationTextBody")
            .SetNotificationId("testNotificationId")
            .Build();
        var actualContent = await request.Content!.ReadAsStringAsync();

        // Assert
        actualContent.Should().BeEquivalentToJson(
            """
            {
                'content': {
                    'accRef': 'did:e:prod.enmeshed.eu:dids:1a7063b5d2c7a8945bf43d',
                    'devicePushIdentifier' : 'DPIaaaaaaaaaaaaaaaaa',
                    'eventName': 'Test',
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
            }
            """
        );
    }

    private record TestPushNotification : IPushNotification
    {
        public required string SomeProperty { get; set; }
    }
}
