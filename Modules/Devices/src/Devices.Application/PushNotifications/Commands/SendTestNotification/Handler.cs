using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Devices.Application.Infrastructure.PushNotifications;
using MediatR;

namespace Backbone.Devices.Application.PushNotifications.Commands.SendTestNotification;

public class Handler : IRequestHandler<SendTestNotificationCommand, Unit>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IPushService _pushService;

    public Handler(IUserContext userContext, IPushService pushService)
    {
        _pushService = pushService;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<Unit> Handle(SendTestNotificationCommand request, CancellationToken cancellationToken)
    {
        await _pushService.SendNotification(_activeIdentity, request.Data, cancellationToken);
        return Unit.Value;
    }
}
