using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Datawallet;
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
        if (request.Data is string s)
            await _pushSenderService.SendNotification(_activeIdentity, new DatawalletModificationsCreatedPushNotification("DVC123x"), s, cancellationToken);
        else
            await _pushSenderService.SendNotification(_activeIdentity, request.Data, "en", cancellationToken);
        return Unit.Value;
    }
}
