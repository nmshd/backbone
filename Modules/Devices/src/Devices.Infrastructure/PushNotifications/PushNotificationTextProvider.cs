using System.Globalization;
using System.Resources;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public class PushNotificationTextProvider : IPushNotificationTextProvider
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly PushNotificationResourceManager _resourceManager;

    public PushNotificationTextProvider(IIdentitiesRepository identitiesRepository, PushNotificationResourceManager resourceManager)
    {
        _identitiesRepository = identitiesRepository;
        _resourceManager = resourceManager;
    }

    public async Task<(string Title, string Body)> GetNotificationTextForDeviceId(Type pushNotificationType, DeviceId deviceId)
    {
        var device = await _identitiesRepository.GetDeviceById(deviceId, CancellationToken.None) ?? throw new Exception("A device with the given id could not be found.");
        var languageCode = device.CommunicationLanguage.Value;

        return GetNotificationTextForLanguage(pushNotificationType, languageCode);
    }

    private (string Title, string Body) GetNotificationTextForLanguage(Type pushNotificationType, string? languageCode)
    {
        var titleKey = $"{pushNotificationType.Name}.Title";
        var bodyKey = $"{pushNotificationType.Name}.Body";

        var culture = languageCode != null ? new CultureInfo(languageCode) : null;

        try
        {
            var title = _resourceManager.GetString(titleKey, culture);
            var body = _resourceManager.GetString(bodyKey, culture);

            if (title.IsNullOrEmpty())
                throw new MissingPushNotificationTextException($"Title for notification type '{pushNotificationType.Name}' not found.");

            if (body.IsNullOrEmpty())
                throw new MissingPushNotificationTextException($"Body for notification type '{pushNotificationType.Name}' not found.");

            return (title, body);
        }
        catch (MissingManifestResourceException)
        {
            throw new MissingPushNotificationTextException($"Title or body for notification type {pushNotificationType.Name} not found.");
        }
    }
}

public class MissingPushNotificationTextException : Exception
{
    public MissingPushNotificationTextException(string message) : base(message)
    {
    }
}
