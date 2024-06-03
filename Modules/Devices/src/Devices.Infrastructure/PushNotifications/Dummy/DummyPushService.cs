using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;

public class DummyPushService : IPushNotificationRegistrationService, IPushNotificationSender
{
    private readonly PushNotificationTextProvider _notificationTextService;
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<DummyPushService> _logger;

    public DummyPushService(PushNotificationTextProvider notificationTextService, IIdentitiesRepository identitiesRepository, ILogger<DummyPushService> logger)
    {
        _notificationTextService = notificationTextService;
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task SendNotification(IdentityAddress recipient, IPushNotification notification, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(recipient, cancellationToken) ?? throw new Exception("Identity not found.");
        foreach (var device in identity.Devices)
        {
            var (title, body) = await _notificationTextService.GetNotificationTextForDeviceId(notification.GetType(), device.Id);
            _logger.LogInformation("Sending push notification to device with id '{deviceId}' of identity with address '{recipient}': {title}, {body}.", recipient, device.Id, title, body);
        }
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
