using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteDeviceRegistration;
internal class Handler : IRequestHandler<DeleteDeviceRegistrationCommand>
{
    private readonly IPushNotificationRegistrationService _pushRegistrationService;
    private readonly DeviceId _activeDevice;

    public Handler(IPushNotificationRegistrationService pushRegistrationService, IUserContext userContext)
    {
        _pushRegistrationService = pushRegistrationService;
        _activeDevice = userContext.GetDeviceId();
    }


    public async Task Handle(DeleteDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        await _pushRegistrationService.DeleteRegistration(_activeDevice, cancellationToken);
    }
}
