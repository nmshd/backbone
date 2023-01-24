using Devices.Application.Infrastructure.PushNotifications;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Devices.Application.PushNotifications.Commands.SendTestNotification;

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
        await _pushService.SendNotificationAsync(_activeIdentity, request.Data);
        return Unit.Value;
    }
}
