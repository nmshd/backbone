using System.Globalization;
using System.Reflection;
using System.Resources;
using Backbone.BuildingBlocks.Domain.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Translations;
using Microsoft.Extensions.Localization;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public class NotificationTextService
{
    private readonly IStringLocalizer _localizer;
    private readonly IIdentitiesRepository _identitiesRepository;
    private const string DEFAULT_LANGUAGE_CODE = "en";

    public NotificationTextService(
        IStringLocalizer<IPushNotificationResource> localizer,
        IIdentitiesRepository identitiesRepository
        )
    {
        _localizer = localizer;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<(string Title, string Body)> GetNotificationTextForDeviceId(IPushNotification pushNotification, DeviceId deviceId)
    {
        //var device = await _identitiesRepository.GetDeviceById(deviceId, CancellationToken.None);
        //var languageCode = device.LanguageCode;
        const string languageCode = DEFAULT_LANGUAGE_CODE;

        return await GetNotificationTextForLanguage(pushNotification, languageCode);
    }

    public async Task<(string Title, string Body)> GetNotificationText(IPushNotification pushNotification) => await GetNotificationTextForLanguage(pushNotification, DEFAULT_LANGUAGE_CODE);

    public async Task<(string Title, string Body)> GetNotificationTextForLanguage(IPushNotification pushNotification, string languageCode)
    {
        var (title, body) = LoadTranslationFromResources(pushNotification, languageCode);
        return title is not null && body is not null ? (title, body) : ("", "");
    }

    private (string? title, string? body) LoadTranslationFromResources(object pushNotification, string languageCode)
    {
        var localizedStrings = GetStrings(pushNotification.GetType(), languageCode).ToArray();
        return (localizedStrings.FindStringEndingWith("Title"), localizedStrings.FindStringEndingWith("Body"));
    }

    public IEnumerable<LocalizedString> GetStrings(Type type, string languageCode = "en")
    {
        var localizedStrings = new List<LocalizedString>();

        lock (CultureInfo.CurrentUICulture)
        {
            var oldCurrentUiCulture = CultureInfo.CurrentCulture;
            var titleKey = $"{type.Name}.Title";
            var bodyKey = $"{type.Name}.Body";

            try
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
                localizedStrings.Add(_localizer[titleKey]);
                localizedStrings.Add(_localizer[bodyKey]);
            }
            catch (SystemException ex) when (ex is ArgumentNullException or MissingManifestResourceException)
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                localizedStrings.Add(_localizer[titleKey]);
                localizedStrings.Add(_localizer[bodyKey]);
            }

            CultureInfo.CurrentUICulture = oldCurrentUiCulture;
        }
        return localizedStrings.ToArray();
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

