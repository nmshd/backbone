using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling.Extensions;
using MediatR;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class Handler : IRequestHandler<UpdateDeviceRegistrationCommand, Unit>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IPushService _pushService;
    private readonly DirectPnsCommunicationOptions _options;
    private readonly DeviceId _activeDevice;

    public Handler(IPushService pushService, IUserContext userContext, IOptions<DirectPnsCommunicationOptions> options)
    {
        _pushService = pushService;
        _options = options.Value;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<Unit> Handle(UpdateDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        var platform = DeserializePlatform(request.Platform);
        var parseHandleResult = PnsHandle.Parse(request.Handle, platform);
        if (parseHandleResult.IsSuccess)
        {
            await _pushService.UpdateRegistration(_activeIdentity, _activeDevice, parseHandleResult.Value, ValidateBundleId(request.AppId, platform), cancellationToken);
        }
        else
        {
            throw new ApplicationException(new ApplicationError(parseHandleResult.Error.Code, parseHandleResult.Error.Message));
        }

        return Unit.Value;
    }

    private string ValidateBundleId(string bundleId, PushNotificationPlatform pushNotificationPlatform)
    {
        if (bundleId.IsNullOrEmpty())
        {
            bundleId = pushNotificationPlatform switch
            {
                PushNotificationPlatform.Apns => _options.Apns.DefaultBundleId,
                PushNotificationPlatform.Fcm => _options.Fcm.DefaultBundleId,
                _ => bundleId
            };
        }

        return bundleId;
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
