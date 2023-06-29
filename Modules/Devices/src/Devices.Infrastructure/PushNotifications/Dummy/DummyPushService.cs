using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
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

    public Task SendNotification(IdentityAddress recipient, object notification)
    {
        _logger.LogInformation($"Sending push notification to '{recipient}'.");
        return Task.CompletedTask;
    }

    public Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle)
    {
        _logger.LogInformation($"Registering for push notifications of Identity '{address}' and device '{deviceId}.");
        return Task.CompletedTask;
    }
}
