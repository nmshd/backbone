using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class Handler : IRequestHandler<UpdateDeviceRegistrationCommand, UpdateDeviceRegistrationResponse>
{
    private const string PRODUCTION_ENVIRONMENT = "Production";
    private const string DEVELOPMENT_ENVIRONMENT = "Development";
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly IPushNotificationRegistrationService _pushRegistrationService;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IPushNotificationRegistrationService pushRegistrationService, IUserContext userContext, IIdentitiesRepository identitiesRepository)
    {
        _pushRegistrationService = pushRegistrationService;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
        _identitiesRepository = identitiesRepository;
    }

    public async Task<UpdateDeviceRegistrationResponse> Handle(UpdateDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_activeIdentity, CancellationToken.None);

        if (identity!.Status == IdentityStatus.ToBeDeleted)
            throw new ApplicationException(ApplicationErrors.Devices.CannotSendNotificationsWhileIdentityIsToBeDeleted());

        var parseHandleResult = PnsHandle.Parse(DeserializePlatform(request.Platform), request.Handle);

        if (parseHandleResult.IsFailure)
            throw new ApplicationException(new ApplicationError(parseHandleResult.Error.Code, parseHandleResult.Error.Message));

        var environment = DeserializeEnvironment(request.Environment ?? PRODUCTION_ENVIRONMENT);
        var devicePushIdentifier = await _pushRegistrationService.UpdateRegistration(_activeIdentity, _activeDevice, parseHandleResult.Value, request.AppId, environment, cancellationToken);

        return new UpdateDeviceRegistrationResponse(devicePushIdentifier);
    }

    private static PushEnvironment DeserializeEnvironment(string environment)
    {
        return environment switch
        {
            DEVELOPMENT_ENVIRONMENT => PushEnvironment.Development,
            PRODUCTION_ENVIRONMENT => PushEnvironment.Production,
            _ => throw new NotSupportedException($"The environment '{environment}' is invalid.")
        };
    }

    private static PushNotificationPlatform DeserializePlatform(string platform)
    {
        return platform switch
        {
            "fcm" => PushNotificationPlatform.Fcm,
            "apns" => PushNotificationPlatform.Apns,
            "dummy" => PushNotificationPlatform.Dummy,
            "sse" => PushNotificationPlatform.Sse,
            _ => throw new NotSupportedException($"The platform '{platform}' is invalid.")
        };
    }
}
