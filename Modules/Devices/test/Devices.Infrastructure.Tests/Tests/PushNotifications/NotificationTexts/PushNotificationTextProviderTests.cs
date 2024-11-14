using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.NotificationTexts;

public class PushNotificationTextProviderTests : AbstractTestsBase
{
    private static readonly string[] SUPPORTED_LANGUAGES = ["en", "de", "pt", "it"];
    public static readonly TheoryData<Type> NOTIFICATION_TYPES_DATA = new(GetNotificationTypes());

    [Fact]
    public void Missing_translation_throws_custom_exception()
    {
        // Arrange
        var notificationTextProvider = CreateNotificationTextProvider();

        // Act
        var acting = () => notificationTextProvider.GetNotificationTextForLanguage(typeof(PushNotificationWithoutExistingTexts), CommunicationLanguage.DEFAULT_LANGUAGE);

        // Assert
        acting.Should().Throw<MissingPushNotificationTextException>();
    }

    [Theory, MemberData(nameof(NOTIFICATION_TYPES_DATA))]
    public void All_push_notifications_have_english_texts(Type notificationType)
    {
        // Arrange
        var notificationTextProvider = CreateNotificationTextProvider();

        // Act
        var acting = () => notificationTextProvider.GetNotificationTextForLanguage(notificationType, CommunicationLanguage.DEFAULT_LANGUAGE);

        // Assert
        acting.Should().NotThrow<MissingPushNotificationTextException>();
    }

    [Theory, ClassData(typeof(AllSupportedLanguagesExceptEnglishCrossJoinedWithNotificationTypes))]
    public void All_push_notifications_have_translations_for_all_supported_languages(CommunicationLanguage language, Type notificationType)
    {
        // Arrange
        var notificationTextProvider = CreateNotificationTextProvider();

        // Act
        var englishText = notificationTextProvider.GetNotificationTextForLanguage(notificationType, CommunicationLanguage.DEFAULT_LANGUAGE);
        var foreignTexts = notificationTextProvider.GetNotificationTextForLanguage(notificationType, language);

        // Assert
        foreignTexts.Should().NotBe(englishText);
    }

    private static PushNotificationTextProvider CreateNotificationTextProvider(PushNotificationResourceManager? resourceManager = null)
    {
        return new PushNotificationTextProvider(resourceManager ?? new PushNotificationResourceManager());
    }

    private static IEnumerable<Type> GetNotificationTypes()
    {
        return
            typeof(TestPushNotification)
                .Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IPushNotification)) && !t.IsInterface);
    }

    private class AllSupportedLanguagesExceptEnglishCrossJoinedWithNotificationTypes : TheoryData<CommunicationLanguage, Type>
    {
        private static readonly CommunicationLanguage[] SUPPORTED_LANGUAGES_EXCEPT_ENGLISH = SUPPORTED_LANGUAGES.Where(l => l != "en").Select(l => CommunicationLanguage.Create(l).Value).ToArray();

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
