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
    private readonly PushNotificationMetrics _metrics;
    private readonly FcmConfiguration _configuration;

    public FirebaseCloudMessagingConnector(FirebaseMessagingFactory firebaseMessagingFactory, IOptions<FcmConfiguration> options, ILogger<FirebaseCloudMessagingConnector> logger,
        PushNotificationMetrics metrics)
    {
        _firebaseMessagingFactory = firebaseMessagingFactory;
        _logger = logger;
        _metrics = metrics;
        _configuration = options.Value;
    }

    public async Task<SendResult> Send(PnsRegistration registration, IPushNotification notification, NotificationText notificationText)
    {
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(registration.IdentityAddress, registration.DevicePushIdentifier, notification);

        return await Send(registration, notificationText, notificationContent, notificationId);
    }

    public Task<SendResult> Send(PnsRegistration registration, NotificationText notificationText, string notificationId)
    {
        return Send(registration, notificationText, null, notificationId);
    }

    private async Task<SendResult> Send(PnsRegistration registration, NotificationText notificationText, NotificationContent? notificationContent, string? notificationId)
    {
        ValidateRegistration(registration);

        var message = new FcmMessageBuilder()
            .AddContent(notificationContent)
            .SetNotificationText(notificationText.Title, notificationText.Body)
            .SetTag(notificationId)
            .SetToken(registration.Handle.Value)
            .Build();

        _logger.Sending();

        return await Send(registration, message, notificationContent?.EventName);
    }

    private async Task<SendResult> Send(PnsRegistration registration, Message message, string? eventName)
    {
        var firebaseMessaging = _firebaseMessagingFactory.CreateForAppId(registration.AppId);
        try
        {
            await firebaseMessaging.SendAsync(message);
            _metrics.IncrementNumberOfSentPushNotifications(eventName, PushNotificationPlatform.Fcm);
            return SendResult.Success(registration.DeviceId);
        }
        catch (FirebaseMessagingException ex)
        {
            var reason = ex.MessagingErrorCode switch
            {
                MessagingErrorCode.InvalidArgument or MessagingErrorCode.Unregistered => ErrorReason.InvalidHandle,
                _ => ErrorReason.Unexpected
            };

            _metrics.IncrementNumberOfSendErrors(eventName, reason, PushNotificationPlatform.Apns);

            return SendResult.Failure(registration.DeviceId, reason);
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
        Message = "Sending push notification...")]
    public static partial void Sending(this ILogger logger);
}
