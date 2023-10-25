﻿using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public interface IPushService
{
    Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, Environment environment, CancellationToken cancellationToken);
    Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken);
    Task SendNotification(IdentityAddress recipient, object notification, CancellationToken cancellationToken);
}
