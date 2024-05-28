using System.Globalization;
using System.Reflection;
using System.Resources;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Translations;
using Microsoft.Extensions.Localization;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public class PushNotificationTextProvider
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private const string DEFAULT_LANGUAGE_CODE = "en";

    public PushNotificationTextProvider(
        IIdentitiesRepository identitiesRepository
        )
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<(string Title, string Body)> GetNotificationTextForDeviceId(Type pushNotificationType, DeviceId deviceId)
    {
        //var device = await _identitiesRepository.GetDeviceById(deviceId, CancellationToken.None);
        //var languageCode = device.LanguageCode;
        const string languageCode = DEFAULT_LANGUAGE_CODE;

        return GetNotificationTextForLanguage(pushNotificationType, languageCode);
    }

    public async Task<(string Title, string Body)> GetNotificationText(Type pushNotificationType) => GetNotificationTextForLanguage(pushNotificationType, DEFAULT_LANGUAGE_CODE);

    public (string Title, string Body) GetNotificationTextForLanguage(Type pushNotificationType, string languageCode)
    {
        var (title, body) = GetStrings(pushNotificationType, languageCode);
        return title is not null && body is not null ? (title, body) : ("", "");
    }

    public (string? title, string? body) GetStrings(Type type, string languageCode)
    {
        var titleKey = $"{type.Name}.Title";
        var bodyKey = $"{type.Name}.Body";

        var rm = new ResourceManager(typeof(IPushNotificationResource).FullName!, GetType().Assembly);

        var title = rm.GetString(titleKey, new CultureInfo(languageCode));
        var body = rm.GetString(bodyKey, new CultureInfo(languageCode));

        return (title, body);
    }
}

public static class TypeExtensions
{
    public static T? GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T?)type.GetCustomAttribute(typeof(T));
    }

    public static string? FindStringEndingWith(this LocalizedString[] localizedStrings, string endsWith)
    {
        return localizedStrings.SingleOrDefault(s => s.Name.EndsWith($".{endsWith}"))?.Value;
    }
}

