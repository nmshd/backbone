using System.Reflection;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Options;
using RestSharp;
using static Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.IServiceCollectionExtensions.DirectPnsCommunicationOptions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class ApplePushNotificationServiceConnector : IPnsConnector
{
    // TODO cache JWT
    private readonly RestClient _client;
    private readonly ApnsOptions _options;

    public ApplePushNotificationServiceConnector(RestClient client, IOptions<ApnsOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var recipients = registrations.Select(r => r.Handle.Value).ToList();

        var (notificationTitle, notificationBody) = GetNotificationText(notification);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(recipient, notification);

        var jwt = Jwt.Create(_options.PrivateKey, _options.KeyId, _options.TeamId);

        var tasks = recipients.Select(device =>
        {
            var request = new ApnsMessageBuilder(_options.Server, _options.AppBundleIdentifier, device, jwt.Value)
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetTag(notificationId)
                .Build();

            return _client.ExecuteAsync(request);
        });
        await Task.WhenAll(tasks);
    }

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }

    private static (string Title, string Body) GetNotificationText(object pushNotification)
    {
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
}

public static class TypeExtensions
{
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T)type.GetCustomAttribute(typeof(T));
    }
}