using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteDeviceRegistration;
internal class Handler : IRequestHandler<DeleteDeviceRegistrationCommand, Unit>
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


    public async Task<Unit> Handle(DeleteDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handle DeleteDeviceRegistrationCommand");

        var parseHandleResult = PnsHandle.Parse(request.Handle, DeserializePlatform(request.Platform));
        if (parseHandleResult.IsSuccess)
        {
            await _pushService.DeleteRegistration(_activeIdentity, _activeDevice, parseHandleResult.Value, request.AppId, cancellationToken);
        }
        else
        {
            throw new ApplicationException(new ApplicationError(parseHandleResult.Error.Code, parseHandleResult.Error.Message));
        }

        return Unit.Value;
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
