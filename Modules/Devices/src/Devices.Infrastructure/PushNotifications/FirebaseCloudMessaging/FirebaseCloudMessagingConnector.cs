﻿using System.Collections.Immutable;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FirebaseAdmin.Messaging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.FirebaseCloudMessaging;
public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessaging _firebaseMessaging;

    public FirebaseCloudMessagingConnector(FirebaseMessaging firebaseMessaging)
    {
        _firebaseMessaging = firebaseMessaging;
    }

    public async Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient,
        object notification)
    {
        var recipients = registrations.Select(r => r.Handle.Value).ToList();
        var recipientsBatches = recipients.Split(500);

        var (notificationTitle, notificationBody) = GetNotificationText(notification);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(recipient, notification);

        foreach (var batch in recipientsBatches)
        {
            var message = new FcmMessageBuilder()
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetTag(notificationId)
                .SetTokens(batch.ToImmutableList())
                .Build();

            await _firebaseMessaging.SendMulticastAsync(message);
        }
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