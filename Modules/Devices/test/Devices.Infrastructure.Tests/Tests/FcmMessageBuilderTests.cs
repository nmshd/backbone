using System;
using System.Text.Json;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Enmeshed.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests;

public class FcmMessageBuilderTests
{
    [Fact]
    public void Test1()
    {
        SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

        var notificationTitle = "someNotificationTextTitle";
        var notificationText = "someNotificationTextBody";
        var message = new FcmMessageBuilder()
            .SetTag(1)
            .SetNotificationText(notificationTitle, notificationText)
            .AddContent(new NotificationContent(IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), new { SomeProperty = "someValue" }))
            .Build();

       message.Notification.Title.Should().Be(notificationTitle);
       message.Notification.Body.Should().Be(notificationText);
       message.Data.Should().ContainKey("_content");
       message.Data.Should().ContainKey("tag");
       message.Data.Should().ContainKey("android_channel_id");
       message.Data["_content"].Should().BeValidJson();

    }
}
