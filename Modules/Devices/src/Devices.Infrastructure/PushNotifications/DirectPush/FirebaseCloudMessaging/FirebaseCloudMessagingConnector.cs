using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Enmeshed.BuildingBlocks.Infrastructure.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;

public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessagingFactory _firebaseMessagingFactory;
    private readonly DirectPnsCommunicationOptions.FcmOptions _options;

    public FirebaseCloudMessagingConnector(FirebaseMessagingFactory firebaseMessagingFactory, IOptions<DirectPnsCommunicationOptions.FcmOptions> options)
    {
        _firebaseMessagingFactory = firebaseMessagingFactory;
        _options = options.Value;
    }

    public async Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var registrationsByAppId = registrations.GroupBy(r => r.AppId)
            .Select(r => new
            {
                AppId = r.Key,
                Handles = r.Select(pnsRegistration =>
                {
                    ValidateRegistration(pnsRegistration);
                    return pnsRegistration.Handle.Value;
                }).ToList()
            });

        foreach (var pnsRegistrations in registrationsByAppId)
        {
            var (notificationTitle, notificationBody) = GetNotificationText(notification);
            var notificationId = GetNotificationId(notification);
            var notificationContent = new NotificationContent(recipient, notification);

            var message = new FcmMessageBuilder()
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetTag(notificationId)
                .SetTokens(pnsRegistrations.Handles.ToImmutableList())
                .Build();

            var firebaseMessaging = _firebaseMessagingFactory.CreateForAppId(pnsRegistrations.AppId);
            await firebaseMessaging.SendMulticastAsync(message);
        }
    }
    
    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_options.HasConfigForAppId(registration.AppId))
            throw new InfrastructureException(GenericInfrastructureErrors.InvalidPushNotificationConfiguration(_options.GetSupportedAppIds()));
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

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }
}

public static class TypeExtensions
{
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T)type.GetCustomAttribute(typeof(T));
    }
}
