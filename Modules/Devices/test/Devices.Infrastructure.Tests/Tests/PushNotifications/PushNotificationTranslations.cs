using System.Resources;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications;
public class PushNotificationTranslationTests
{
    [Fact]
    public void AllPushNotificationsHaveEnglishText()
    {
        var notificationTypesWithMissingTranslations = new List<Type>();

        var notificationTextService = CreateNotificationTextService();
        var notificationTypes = GetNotificationTypes().ToArray();

        foreach (var notificationType in notificationTypes)
        {
            try
            {
                notificationTextService.GetNotificationTextWithDefaultLanguage(notificationType);
            }
            catch (MissingManifestResourceException)
            {
                notificationTypesWithMissingTranslations.Add(notificationType);
            }
        }

        notificationTypesWithMissingTranslations.Should().BeEmpty();
    }
        
    private static PushNotificationTextProvider CreateNotificationTextService()
    {
        return new PushNotificationTextProvider(A.Dummy<IIdentitiesRepository>());
    }

    private static IEnumerable<Type> GetNotificationTypes()
    {
        return typeof(TestPushNotification).Assembly.GetTypes().Where(t => typeof(IPushNotification).IsAssignableFrom(t) && !t.IsInterface);
    }
}

