﻿using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class Handler : IRequestHandler<UpdateDeviceRegistrationCommand, Unit>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IPushService _pushService;
    private readonly DeviceId _activeDevice;

    public Handler(IPushService pushService, IUserContext userContext)
    {
        _pushService = pushService;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<Unit> Handle(UpdateDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        var handle = PnsHandle.Parse(request.Handle, PlatformParser(request.Platform)).Value;
        await _pushService.UpdateRegistration(_activeIdentity, _activeDevice, handle);
        return Unit.Value;
    }

    private PushNotificationPlatform PlatformParser(string platform)
    {
        return platform switch
        {
            "fcm" => PushNotificationPlatform.Fcm,
            "apns" => PushNotificationPlatform.Apns,
            _ => throw new ArgumentException(),
        };
    }
}
