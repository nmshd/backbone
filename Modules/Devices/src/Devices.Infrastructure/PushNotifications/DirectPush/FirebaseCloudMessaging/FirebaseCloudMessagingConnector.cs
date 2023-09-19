using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Enmeshed.BuildingBlocks.Infrastructure.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;

public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessagingFactory _firebaseMessagingFactory;
    private readonly ILogger<FirebaseCloudMessagingConnector> _logger;
    private readonly DirectPnsCommunicationOptions.FcmOptions _options;

    public FirebaseCloudMessagingConnector(FirebaseMessagingFactory firebaseMessagingFactory, IOptions<DirectPnsCommunicationOptions.FcmOptions> options,
        ILogger<FirebaseCloudMessagingConnector> logger)
    {
        _firebaseMessagingFactory = firebaseMessagingFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<SendResult>> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var registrationsByAppId = registrations.GroupBy(r => r.AppId)
            .Select(r => new
            {
                DeviceIds = r.Select(pnsRegistration => pnsRegistration.DeviceId).ToList(),
                AppId = r.Key,
                Handles = r.Select(pnsRegistration =>
                {
                    ValidateRegistration(pnsRegistration);
                    return pnsRegistration.Handle.Value;
                }).ToList()
            });

        var tasks = registrationsByAppId.Select(async pnsRegistrations =>
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

            _logger.LogDebug("Sending push notification (type '{eventName}') to '{address}' with handles '{handle}'.", notificationContent.EventName, recipient,
                string.Join(",", pnsRegistrations.Handles));

            var firebaseMessaging = _firebaseMessagingFactory.CreateForAppId(pnsRegistrations.AppId);
            var response = await firebaseMessaging.SendMulticastAsync(message);
            return MapResponse(response, pnsRegistrations.DeviceIds);
        });

        return (await Task.WhenAll(tasks)).ToList().SelectMany(list => list).ToList();
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_options.HasConfigForAppId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_options.GetSupportedAppIds()));
    }

    private List<SendResult> MapResponse(BatchResponse batchResponse, IReadOnlyList<DeviceId> devices)
    {
        var sendResults = new List<SendResult>();
        for (var index = 0; index < batchResponse.Responses.Count; index++)
        {
            var response = batchResponse.Responses[index];
            var deviceId = devices[index];
            if (response.IsSuccess)
            {
                sendResults.Add(SendResult.Success());
            }
            else
            {
                sendResults.Add(response.Exception.MessagingErrorCode is MessagingErrorCode.InvalidArgument or MessagingErrorCode.Unregistered
                    ? SendResult.Failure(deviceId, SendResult.FailureReason.InvalidHandle)
                    : SendResult.Failure(deviceId, SendResult.FailureReason.Unexpected, response.Exception.Message));
            }
        }

        return sendResults;
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
