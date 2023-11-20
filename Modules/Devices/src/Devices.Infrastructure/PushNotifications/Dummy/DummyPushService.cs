using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;

public class DummyPushService : IPushNotificationRegistrationService, IPushNotificationSender
{
    private readonly ILogger<DummyPushService> _logger;

    public DummyPushService(ILogger<DummyPushService> logger)
    {
        _logger = logger;
    }

    public Task SendNotification(IdentityAddress recipient, object notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending push notification to '{recipient}'.", recipient);
        return Task.CompletedTask;
    }

    public Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, PushEnvironment environment, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering for push notifications of Identity '{address}' and device '{deviceId}.", address, deviceId);
        return Task.CompletedTask;
    }

    public Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unregistering from push notifications with device '{deviceId}.", deviceId);
        return Task.CompletedTask;
    }
}
