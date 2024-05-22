using System.Globalization;
using System.Reflection;
using System.Resources;
using Backbone.BuildingBlocks.Domain.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.Localization;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public class NotificationTextService
{
    private readonly IStringLocalizer _localizer;
    private readonly IIdentitiesRepository _identitiesRepository;
    private const string DEFAULT_LANGUAGE_CODE = "en";

    public NotificationTextService(
        IStringLocalizer<IPushNotification> localizer,
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
        var localizedStrings = GetStrings(languageCode, pushNotification.GetType()).ToArray();
        return (localizedStrings.FindStringEndingWith("Title"), localizedStrings.FindStringEndingWith("Body"));
    }

    public IEnumerable<LocalizedString> GetStrings(string languageCode = "en", Type? type = null)
    {
        var oldCurrentUiCulture = CultureInfo.CurrentCulture;
        IList<LocalizedString> localizedStrings;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
            localizedStrings = _localizer.GetAllStrings().ToList();
        }
        catch (SystemException ex) when (ex is ArgumentNullException or MissingManifestResourceException)
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            localizedStrings = _localizer.GetAllStrings().ToList();
        }

        CultureInfo.CurrentUICulture = oldCurrentUiCulture;

        // try to find a way to not use currentuiculture
        return type is null ? localizedStrings : localizedStrings.Where(ls => ls.Name.StartsWith(type.Name)).ToArray();
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

