using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.SendTestNotification;

public class Handler : IRequestHandler<SendTestNotificationCommand, Unit>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IPushNotificationSender _pushSenderService;

    public Handler(IUserContext userContext, IPushNotificationSender pushSenderService)
    {
        _pushSenderService = pushSenderService;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<Unit> Handle(SendTestNotificationCommand request, CancellationToken cancellationToken)
    {
        await _pushSenderService.SendNotification(
            new TestPushNotification { Data = request.Data },
            SendPushNotificationFilter.AllDevicesOf(_activeIdentity),
            cancellationToken
        );
        return Unit.Value;
    }
}
