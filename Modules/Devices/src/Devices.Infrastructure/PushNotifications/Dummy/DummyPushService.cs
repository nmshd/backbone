using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;

public class DummyPushService : IPushNotificationRegistrationService, IPushNotificationSender
{
    private readonly NotificationTextService _notificationTextService;
    private readonly ILogger<DummyPushService> _logger;

    public DummyPushService(NotificationTextService notificationTextService, ILogger<DummyPushService> logger)
    {
        _notificationTextService = notificationTextService;
        _logger = logger;
    }

    public Task SendNotification(IdentityAddress recipient, object notification, string languageCode, CancellationToken cancellationToken)
    {
        var (title, body) = _notificationTextService.GetNotificationText(notification, languageCode);
        _logger.LogInformation("Sending push notification to '{recipient}': {title}, {body}.", recipient, title, body);
        return Task.CompletedTask;
    }

    public Task<DevicePushIdentifier> UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, PushEnvironment environment, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering for push notifications of Identity '{address}' and device '{deviceId}.", address, deviceId);
        return Task.FromResult(DevicePushIdentifier.New());
    }

    public Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unregistering from push notifications with device '{deviceId}.", deviceId);
        return Task.FromResult(DevicePushIdentifier.New());
    }
}
