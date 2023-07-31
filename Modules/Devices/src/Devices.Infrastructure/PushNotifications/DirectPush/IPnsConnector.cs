﻿using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public interface IPnsConnector
{
    Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification);
}
