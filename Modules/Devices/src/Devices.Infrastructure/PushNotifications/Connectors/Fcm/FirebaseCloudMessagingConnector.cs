using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;

public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessagingFactory _firebaseMessagingFactory;
    private readonly IPushNotificationTextProvider _notificationTextProvider;
    private readonly ILogger<FirebaseCloudMessagingConnector> _logger;
    private readonly FcmOptions _options;

    public FirebaseCloudMessagingConnector(FirebaseMessagingFactory firebaseMessagingFactory, IOptions<FcmOptions> options,
        IPushNotificationTextProvider notificationTextProvider, ILogger<FirebaseCloudMessagingConnector> logger)
    {
        _firebaseMessagingFactory = firebaseMessagingFactory;
        _notificationTextProvider = notificationTextProvider;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IPushNotification notification)
    {
        var registrationsArray = registrations as PnsRegistration[] ?? registrations.ToArray();

        ValidateRegistrations(registrationsArray);

        var sendResults = new SendResults();

        var tasks = registrationsArray.Select(r => SendNotification(r, notification, sendResults));

        await Task.WhenAll(tasks);
        return sendResults;
    }

    private async Task SendNotification(PnsRegistration registration, IPushNotification notification, SendResults sendResults)
    {
        var (notificationTitle, notificationBody) = await _notificationTextProvider.GetNotificationTextForDeviceId(notification.GetType(), registration.DeviceId);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(registration.IdentityAddress, registration.DevicePushIdentifier, notification);

        var message = new FcmMessageBuilder()
            .AddContent(notificationContent)
            .SetNotificationText(notificationTitle, notificationBody)
            .SetTag(notificationId)
            .SetToken(registration.Handle.Value)
            .Build();

        _logger.Sending(notificationContent.EventName, registration.DeviceId, registration.IdentityAddress, registration.Handle.Value);

        var firebaseMessaging = _firebaseMessagingFactory.CreateForAppId(registration.AppId);
        try
        {
            await firebaseMessaging.SendAsync(message);
            sendResults.AddSuccess(registration.DeviceId);
        }
        catch (FirebaseMessagingException ex)
        {
            switch (ex.MessagingErrorCode)
            {
                case MessagingErrorCode.InvalidArgument or MessagingErrorCode.Unregistered:
                    sendResults.AddFailure(registration.DeviceId, ErrorReason.InvalidHandle);
                    break;
                default:
                    sendResults.AddFailure(registration.DeviceId, ErrorReason.Unexpected, ex.Message);
                    break;
            }
        }
    }

    private void ValidateRegistrations(IEnumerable<PnsRegistration> registrations)
    {
        foreach (var registration in registrations)
        {
            ValidateRegistration(registration);
        }
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_options.HasConfigForAppId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_options.GetSupportedAppIds()));
    }

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }
}

internal static partial class FirebaseCloudMessagingConnectorLogs
{
    [LoggerMessage(
        EventId = 227730,
        EventName = "FirebaseCloudMessagingConnector.Sending",
        Level = LogLevel.Debug,
        Message = "Sending push notification (type '{eventName}') to device '{deviceId}' of '{address}' with handle '{handle}'.")]
    public static partial void Sending(this ILogger logger, string eventName, string deviceId, string address, string handle);
}
