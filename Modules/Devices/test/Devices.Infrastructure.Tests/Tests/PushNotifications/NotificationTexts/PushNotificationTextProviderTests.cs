using System.Globalization;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.NotificationTexts;

public class PushNotificationTextProviderTests : AbstractTestsBase
{
    private static readonly string[] SUPPORTED_LANGUAGES = ["en", "de", "pt", "it"];
    public static readonly TheoryData<Type> NOTIFICATION_TYPES_DATA = new(GetNotificationTypes());

    [Fact]
    public void Empty_translation_throws_custom_exception()
    {
        // Arrange
        var device = TestDataGenerator.CreateDevice();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeResourceManager = A.Fake<PushNotificationResourceManager>();

        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(device.Id, A<CancellationToken>._, A<bool>._)).Returns(device);
        A.CallTo(() => fakeResourceManager.GetString(A<string>._, A<CultureInfo>._)).Returns("");

        var notificationTextProvider = CreateNotificationTextProvider(fakeIdentitiesRepository, fakeResourceManager);

        // Act
        var acting = async () => await notificationTextProvider.GetNotificationTextForDeviceId(typeof(PushNotificationWithoutExistingTexts), device.Id);

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

        var notificationTextProvider = CreateNotificationTextProvider(fakeIdentitiesRepository);

        // Act
        var acting = async () => await notificationTextProvider.GetNotificationTextForDeviceId(typeof(PushNotificationWithoutExistingTexts), device.Id);

        // Assert
        acting.Should().AwaitThrowAsync<MissingPushNotificationTextException, (string PageTitle, string Body)>();
    }

    [Theory, MemberData(nameof(NOTIFICATION_TYPES_DATA))]
    public async Task All_push_notifications_have_english_texts(Type notificationType)
    {
        // Arrange
        var device = TestDataGenerator.CreateDevice();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(device.Id, A<CancellationToken>._, A<bool>._)).Returns(device);

        var notificationTextProvider = CreateNotificationTextProvider(fakeIdentitiesRepository);

        // Act
        var acting = async () => await notificationTextProvider.GetNotificationTextForDeviceId(notificationType, device.Id);

        // Assert
        await acting.Should().NotThrowAsync<MissingPushNotificationTextException>();
    }

    [Theory, ClassData(typeof(AllSupportedLanguagesExceptEnglishCrossJoinedWithNotificationTypes))]
    public async Task All_push_notifications_have_translations_for_all_supported_languages(string language, Type notificationType)
    {
        // Arrange
        var englishDevice = TestDataGenerator.CreateDevice();
        var foreignDevice = TestDataGenerator.CreateDevice(language);

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(englishDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(englishDevice);
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(foreignDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(foreignDevice);

        var notificationTextProvider = CreateNotificationTextProvider(fakeIdentitiesRepository);

        // Act
        var englishText = await notificationTextProvider.GetNotificationTextForDeviceId(notificationType, englishDevice.Id);
        var foreignText = await notificationTextProvider.GetNotificationTextForDeviceId(notificationType, foreignDevice.Id);

        // Assert
        foreignText.Should().NotBe(englishText);
    }

    private static PushNotificationTextProvider CreateNotificationTextProvider(IIdentitiesRepository? fakeIdentitiesRepository = null, PushNotificationResourceManager? resourceManager = null)
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

    private class AllSupportedLanguagesExceptEnglishCrossJoinedWithNotificationTypes : TheoryData<string, Type>
    {
        private static readonly string[] SUPPORTED_LANGUAGES_EXCEPT_ENGLISH = SUPPORTED_LANGUAGES.Where(l => l != "en").ToArray();

        public AllSupportedLanguagesExceptEnglishCrossJoinedWithNotificationTypes()
        {
            var items = GetNotificationTypes()
                .SelectMany(_ => SUPPORTED_LANGUAGES_EXCEPT_ENGLISH, (notificationType, language) => new { Language = language, NotificationType = notificationType });

            foreach (var item in items)
            {
                Add(item.Language, item.NotificationType);
            }
        }
    }
}

public record PushNotificationWithoutExistingTexts : IPushNotification;
