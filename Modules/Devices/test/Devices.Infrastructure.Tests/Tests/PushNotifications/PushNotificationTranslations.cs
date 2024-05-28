﻿using Backbone.BuildingBlocks.Domain.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using System.Resources;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications;
public class PushNotificationTranslations
{
    [Fact]
    public async Task AllPushNotificationsHaveEnglishText()
    {
        var notificationTypesWithMissingTranslations = new List<Type>();

        var notificationTextService = CreateNotificationTextService();
        var notificationTypes = GetNotificationTypes().ToArray();

        foreach (var notificationType in notificationTypes)
        {
            try
            {
                await notificationTextService.GetNotificationText(notificationType);
            }
            catch (MissingManifestResourceException)
            {
                notificationTypesWithMissingTranslations.Add(notificationType);
            }
        }

        notificationTypesWithMissingTranslations.Should().BeEmpty();
    }

    private static NotificationTextService CreateNotificationTextService()
    {
        return new NotificationTextService(A.Dummy<IIdentitiesRepository>());
    }

    private static IEnumerable<Type> GetNotificationTypes()
    {
        return typeof(TestPushNotification).Assembly.GetTypes().Where(t => typeof(IPushNotification).IsAssignableFrom(t) && !t.IsInterface);
    }
}

