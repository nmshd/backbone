﻿using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class Handler : IRequestHandler<UpdateDeviceRegistrationCommand, Unit>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IPushService _pushService;
    private readonly DeviceId _activeDevice;
    private const string PRODUCTION_ENVIRONMENT = "Production";
    private const string DEVELOPMENT_ENVIRONMENT = "Development";

    public Handler(IPushService pushService, IUserContext userContext)
    {
        _pushService = pushService;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<Unit> Handle(UpdateDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        var parseHandleResult = PnsHandle.Parse(request.Handle, DeserializePlatform(request.Platform));
        if (parseHandleResult.IsSuccess)
        {
            await _pushService.UpdateRegistration(_activeIdentity, _activeDevice, parseHandleResult.Value, request.AppId, DeserializeEnvironment(request.Environment ?? PRODUCTION_ENVIRONMENT), cancellationToken);
        }
        else
        {
            throw new ApplicationException(new ApplicationError(parseHandleResult.Error.Code, parseHandleResult.Error.Message));
        }

        return Unit.Value;
    }

    private static Environment DeserializeEnvironment(string environment)
    {
        return environment switch
        {
            DEVELOPMENT_ENVIRONMENT => Environment.Development,
            PRODUCTION_ENVIRONMENT => Environment.Production,
            _ => throw new NotImplementedException($"The environment '{environment}' is invalid.")
        };
    }

    private static PushNotificationPlatform DeserializePlatform(string platform)
    {
        return platform switch
        {
            "fcm" => PushNotificationPlatform.Fcm,
            "apns" => PushNotificationPlatform.Apns,
            _ => throw new NotImplementedException($"The platform '{platform}' is invalid.")
        };
    }
}
