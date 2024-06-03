using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.Responses;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;

public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessagingFactory _firebaseMessagingFactory;
    private readonly PushNotificationTextProvider _notificationTextService;
    private readonly ILogger<FirebaseCloudMessagingConnector> _logger;
    private readonly DirectPnsCommunicationOptions.FcmOptions _options;

    public FirebaseCloudMessagingConnector(FirebaseMessagingFactory firebaseMessagingFactory, IOptions<DirectPnsCommunicationOptions.FcmOptions> options,
        PushNotificationTextProvider notificationTextService,
        ILogger<FirebaseCloudMessagingConnector> logger)
    {
        _firebaseMessagingFactory = firebaseMessagingFactory;
        _notificationTextService = notificationTextService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, IPushNotification notification)
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
        var (notificationTitle, notificationBody) = await _notificationTextService.GetNotificationTextForDeviceId(notification.GetType(), registration.DeviceId);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(registration.IdentityAddress, registration.DevicePushIdentifier, notification);

        var message = new FcmMessageBuilder()
            .AddContent(notificationContent)
            .SetNotificationText(notificationTitle, notificationBody)
            .SetTag(notificationId)
            .SetToken(registration.Handle.Value)
            .Build();

        _logger.LogDebug("Sending push notification (type '{eventName}') to device '{deviceId}' of '{address}' with handle '{handle}'.",
            notificationContent.EventName, registration.DeviceId, registration.IdentityAddress, registration.Handle.Value);

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
