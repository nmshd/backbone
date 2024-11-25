using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Device;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UseBackupDevice;

public class Handler : IRequestHandler<UseBackupDeviceCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationSender _pushNotificationSender;

    public Handler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender)
    {
        _identitiesRepository = identitiesRepository;
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(UseBackupDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = (await _identitiesRepository.GetDeviceById(DeviceId.Parse(request.DeviceId), cancellationToken))!;

        device.MarkAsBackupDeviceUsed();

        await _identitiesRepository.Update(device, cancellationToken);
        await _pushNotificationSender.SendNotification(new BackupDeviceUsedPushNotification(), SendPushNotificationFilter.AllDevicesOfExcept(device.IdentityAddress, device.Id), cancellationToken);
    }
}
