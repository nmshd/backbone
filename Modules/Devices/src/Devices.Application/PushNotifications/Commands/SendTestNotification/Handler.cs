using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.SendTestNotification;

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
