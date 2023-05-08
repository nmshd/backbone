using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;

public class DummyPushService : IPushService
{
    private readonly ILogger<DummyPushService> _logger;

    public DummyPushService(ILogger<DummyPushService> logger)
    {
        _logger = logger;
    }

    public Task SendNotificationAsync(IdentityAddress recipient, object notification)
    {
        _logger.LogInformation($"Sending push notification to '{recipient}'.");
        return Task.CompletedTask;
    }

    public Task RegisterDeviceAsync(IdentityAddress identity, DeviceRegistration registration)
    {
        _logger.LogInformation($"Registering for push notifications of Identity '{identity}' and device '{registration.InstallationId}.");
        return Task.CompletedTask;
    }
}
