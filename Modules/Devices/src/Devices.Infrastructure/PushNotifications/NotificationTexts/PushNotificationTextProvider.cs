using System.Globalization;
using System.Resources;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;

public class PushNotificationTextProvider : IPushNotificationTextProvider
{
    private readonly PushNotificationResourceManager _resourceManager;

    public PushNotificationTextProvider(PushNotificationResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public Dictionary<CommunicationLanguage, NotificationText> GetNotificationTextsForLanguages(Type pushNotificationType, IEnumerable<CommunicationLanguage> languages)
    {
        return languages.ToDictionary(
            language => language,
            language => GetNotificationTextForLanguage(pushNotificationType, language)
        );
    }

    public NotificationText GetNotificationTextForLanguage(Type pushNotificationType, CommunicationLanguage languageCode)
    {
        return GetNotificationTextFromResourceManager(pushNotificationType, languageCode);
    }

    private NotificationText GetNotificationTextFromResourceManager(Type pushNotificationType, CommunicationLanguage languageCode)
    {
        var pushNotificationTypeName = pushNotificationType.Name;

        var titleKey = $"{pushNotificationTypeName}.Title";
        var bodyKey = $"{pushNotificationTypeName}.Body";

        var culture = new CultureInfo(languageCode.Value);

        try
        {
            var title = _resourceManager.GetString(titleKey, culture);
            var body = _resourceManager.GetString(bodyKey, culture);

            if (title == null)
                throw new MissingPushNotificationTextException($"Title for notification type '{pushNotificationTypeName}' not found.");

            if (body == null)
                throw new MissingPushNotificationTextException($"Body for notification type '{pushNotificationTypeName}' not found.");

            return new NotificationText(title, body);
        }
        catch (MissingManifestResourceException)
        {
            throw new MissingPushNotificationTextException($"Title or body for notification type {pushNotificationTypeName} not found.");
        }
    }
}

public class MissingPushNotificationTextException(string message) : Exception(message);
