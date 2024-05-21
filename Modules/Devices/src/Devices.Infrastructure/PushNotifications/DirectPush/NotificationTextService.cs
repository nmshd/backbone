using System.Globalization;
using System.Reflection;
using System.Resources;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public class NotificationTextService
{
    private readonly IStringLocalizer _localizer;

    public NotificationTextService(IStringLocalizer<IPushNotification> localizer)
    {
        _localizer = localizer;
    }

    public (string Title, string Body) GetNotificationText(object pushNotification, string languageCode = "en")
    {
        var (title, body) = LoadTranslationFromResources(pushNotification, languageCode);

        if (title is not null && body is not null)
        {
            return (title, body);
        }

        switch (pushNotification)
        {
            case null:
                return ("", "");
            case JsonElement jsonElement:
            {
                var notification = jsonElement.Deserialize<NotificationTextAttribute>();
                return notification == null ? ("", "") : (notification.Title, notification.Body);
            }
            default:
            {
                var attribute = pushNotification.GetType().GetCustomAttribute<NotificationTextAttribute>();
                return attribute == null ? ("", "") : (attribute.Title, attribute.Body);
            }
        }
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

