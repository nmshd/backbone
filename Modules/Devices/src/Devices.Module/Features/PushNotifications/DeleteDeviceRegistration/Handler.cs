using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Abstractions.PushNotifications;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.DeleteDeviceRegistration;

public class Handler : IRequestHandler<Command>
{
    private readonly IPushNotificationRegistrationService _pushRegistrationService;
    private readonly DeviceId _activeDevice;

    public Handler(IPushNotificationRegistrationService pushRegistrationService, IUserContext userContext)
    {
        _pushRegistrationService = pushRegistrationService;
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _pushRegistrationService.DeleteRegistration(_activeDevice, cancellationToken);
    }
}
