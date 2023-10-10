using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteDeviceRegistration;
internal class Handler : IRequestHandler<DeleteDeviceRegistrationCommand>
{
    private readonly IPushService _pushService;
    private readonly DeviceId _activeDevice;

    public Handler(IPushService pushService, IUserContext userContext)
    {
        _pushService = pushService;
        _activeDevice = userContext.GetDeviceId();
    }


    public async Task Handle(DeleteDeviceRegistrationCommand request, CancellationToken cancellationToken)
    {
        await _pushService.DeleteRegistration(_activeDevice, cancellationToken);
    }
}
