using System.Globalization;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications;

public class PushNotificationTextProviderTests : AbstractTestsBase
{
    [Fact]
    public void Empty_translation_throws_custom_exception()
    {
        // Arrange
        var device = TestDataGenerator.CreateDevice();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeResourceManager = A.Fake<PushNotificationResourceManager>();

        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(device.Id, A<CancellationToken>._, A<bool>._)).Returns(device);
        A.CallTo(() => fakeResourceManager.GetString(A<string>._, A<CultureInfo>._)).Returns("");

        var notificationTextService = CreateNotificationTextService(fakeIdentitiesRepository, fakeResourceManager);

        // Act
        var acting = async () => await notificationTextService.GetNotificationTextForDeviceId(typeof(PushNotificationWithoutExistingTexts), device.Id);

        // Assert
        acting.Should().AwaitThrowAsync<MissingPushNotificationTextException, (string PageTitle, string Body)>();
    }

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

    [Theory]
    [InlineData("pt")]
    public async Task AllPushNotificationsHaveForeignText(string language)
    {
        // Arrange
        var notificationTypesWithMissingTranslations = new List<Type>();

        var englishDevice = TestDataGenerator.CreateDevice();
        var foreignDevice = TestDataGenerator.CreateDevice();
        foreignDevice.CommunicationLanguage = CommunicationLanguage.Create(language).Value;

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(englishDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(englishDevice);
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(foreignDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(foreignDevice);

        var notificationTextService = CreateNotificationTextService(fakeIdentitiesRepository);
        var notificationTypes = GetNotificationTypes().ToArray();

        // Act
        foreach (var notificationType in notificationTypes)
        {
            try
            {
                var englishText = await notificationTextService.GetNotificationTextForDeviceId(notificationType, englishDevice.Id);
                var foreignText = await notificationTextService.GetNotificationTextForDeviceId(notificationType, foreignDevice.Id);

                if (englishText == foreignText)
                {
                    notificationTypesWithMissingTranslations.Add(notificationType);
                }
            }
            catch (MissingPushNotificationTextException)
            {
                notificationTypesWithMissingTranslations.Add(notificationType);
            }
        }

        // Assert
        notificationTypesWithMissingTranslations.Should().BeEmpty();
    }

    private static PushNotificationTextProvider CreateNotificationTextService(IIdentitiesRepository? fakeIdentitiesRepository = null, PushNotificationResourceManager? resourceManager = null)
    {
        return new PushNotificationTextProvider(
            fakeIdentitiesRepository ?? A.Fake<IIdentitiesRepository>(),
            resourceManager ?? new PushNotificationResourceManager()
        );
    }

    private static IEnumerable<Type> GetNotificationTypes()
    {
        return typeof(TestPushNotification).Assembly.GetTypes().Where(t => typeof(IPushNotification).IsAssignableFrom(t) && !t.IsInterface);
    }
}

public record PushNotificationWithoutExistingTexts : IPushNotification;
