using System.Reflection;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.IServiceCollectionExtensions.DirectPnsCommunicationOptions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class ApplePushNotificationServiceConnector : IPnsConnector
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly ILogger<ApplePushNotificationServiceConnector> _logger;
    private readonly ApnsOptions _options;
    private static Jwt _jwt;

    public ApplePushNotificationServiceConnector(IHttpClientFactory httpClientFactory, IOptions<ApnsOptions> options, IJwtGenerator jwtGenerator, ILogger<ApplePushNotificationServiceConnector> logger)
    {
        _httpClientFactory = httpClientFactory;
        _jwtGenerator = jwtGenerator;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var recipients = registrations.Select(r => r.Handle.Value).ToList();

        var (notificationTitle, notificationBody) = GetNotificationText(notification);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(recipient, notification);

        if (_jwt == null)
            _jwt = _jwtGenerator.Generate(_options.PrivateKey, _options.KeyId, _options.TeamId);

        if (_jwt.IsExpired())
        {
            lock (_jwt)
            {
                if (_jwt.IsExpired())
                    _jwt = _jwtGenerator.Generate(_options.PrivateKey, _options.KeyId, _options.TeamId);
            }
        }

        var client = _httpClientFactory.CreateClient();
        var tasks = recipients.Select(device =>
        {
            var request = new ApnsMessageBuilder(_options.AppBundleIdentifier, $"{_options.Server}{device}", _jwt.Value)
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetTag(notificationId)
                .Build();

            return (request: client.SendAsync(request), deviceId: device);
        }).ToList();

        var responses = await Task.WhenAll(tasks.Select(x => x.request));
        for (var index = 0; index < responses.Length; index++)
        {
            var response = responses[index];
            if (response is { IsSuccessStatusCode: false })
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!responseContent.IsNullOrEmpty())
                {
                    _logger.LogError(
                        "The following error occurred while trying to send the notification for device {device}: {responseContent}",
                        tasks.ElementAt(index).deviceId, responseContent);
                }
                else
                {
                    _logger.LogError("An unknown error occurred while trying to send the notification for device {device}.", tasks.ElementAt(index).deviceId);
                }
            }
        }
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