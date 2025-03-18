using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;

public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessagingFactory _firebaseMessagingFactory;
    private readonly ILogger<FirebaseCloudMessagingConnector> _logger;
    private readonly FcmConfiguration _configuration;

    public FirebaseCloudMessagingConnector(FirebaseMessagingFactory firebaseMessagingFactory, IOptions<FcmConfiguration> options, ILogger<FirebaseCloudMessagingConnector> logger)
    {
        _firebaseMessagingFactory = firebaseMessagingFactory;
        _logger = logger;
        _configuration = options.Value;
    }

    public async Task<SendResult> Send(PnsRegistration registration, IPushNotification notification, NotificationText notificationText)
    {
        ValidateRegistration(registration);

        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(registration.IdentityAddress, registration.DevicePushIdentifier, notification);

        var message = new FcmMessageBuilder()
            .AddContent(notificationContent)
            .SetNotificationText(notificationText.Title, notificationText.Body)
            .SetTag(notificationId)
            .SetToken(registration.Handle.Value)
            .Build();

        _logger.Sending(notificationContent.EventName);

        var firebaseMessaging = _firebaseMessagingFactory.CreateForAppId(registration.AppId);
        try
        {
            await firebaseMessaging.SendAsync(message);
            return SendResult.Success(registration.DeviceId);
        }
        catch (FirebaseMessagingException ex)
        {
            return ex.MessagingErrorCode switch
            {
                MessagingErrorCode.InvalidArgument or MessagingErrorCode.Unregistered => SendResult.Failure(registration.DeviceId, ErrorReason.InvalidHandle),
                _ => SendResult.Failure(registration.DeviceId, ErrorReason.Unexpected, ex.Message)
            };
        }
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_configuration.HasConfigForAppId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_configuration.GetSupportedAppIds()));
    }

    private static string? GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? null;
    }
}

internal static partial class FirebaseCloudMessagingConnectorLogs
{
    [LoggerMessage(
        EventId = 227730,
        EventName = "FirebaseCloudMessagingConnector.Sending",
        Level = LogLevel.Debug,
        Message = "Sending push notification (type '{eventName}').")]
    public static partial void Sending(this ILogger logger, string eventName);
}
