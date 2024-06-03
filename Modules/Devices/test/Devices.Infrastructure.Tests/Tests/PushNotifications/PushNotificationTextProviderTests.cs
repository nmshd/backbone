using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications;

public class PushNotificationTextProviderTests : AbstractTestsBase
{
    /*
     * - "en" is used in case the device's languages is not supported
     * - Throws if the key is there but the value is empty (use a fake ResourceManager)
     */

    [Fact]
    public void Missing_translation_throws_custom_exception()
    {
        // Arrange
        var device = TestDataGenerator.CreateDevice();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(device.Id, A<CancellationToken>._, A<bool>._)).Returns(device);

        var notificationTextService = CreateNotificationTextService(fakeIdentitiesRepository);

        // Act
        var acting = async () => await notificationTextService.GetNotificationTextForDeviceId(typeof(PushNotificationWithoutExistingTexts), device.Id);

        // Assert
        acting.Should().AwaitThrowAsync<MissingPushNotificationTextException, (string PageTitle, string Body)>();
    }

    [Fact]
    public async Task AllPushNotificationsHaveEnglishText()
    {
        // Arrange
        var notificationTypesWithMissingTranslations = new List<Type>();

        var device = TestDataGenerator.CreateDevice();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(device.Id, A<CancellationToken>._, A<bool>._)).Returns(device);

        var notificationTextService = CreateNotificationTextService(fakeIdentitiesRepository);
        var notificationTypes = GetNotificationTypes().ToArray();

        // Act
        foreach (var notificationType in notificationTypes)
        {
            try
            {
                await notificationTextService.GetNotificationTextForDeviceId(notificationType, device.Id);
            }
            catch (MissingPushNotificationTextException)
            {
                notificationTypesWithMissingTranslations.Add(notificationType);
            }
        }

        // Assert
        notificationTypesWithMissingTranslations.Should().BeEmpty();
    }

    private static PushNotificationTextProvider CreateNotificationTextService(IIdentitiesRepository? fakeIdentitiesRepository = null)
    {
        return new PushNotificationTextProvider(fakeIdentitiesRepository ?? A.Fake<IIdentitiesRepository>());
    }

    private static IEnumerable<Type> GetNotificationTypes()
    {
        return typeof(TestPushNotification).Assembly.GetTypes().Where(t => typeof(IPushNotification).IsAssignableFrom(t) && !t.IsInterface);
    }
}

public record PushNotificationWithoutExistingTexts : IPushNotification;
