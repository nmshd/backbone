using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteRegistrationsByIdentityAddress;
public class Handler(IPushNotificationRegistrationService pnrService) : IRequestHandler<DeleteRegistrationsByIdentityAddressCommand>
{
    private readonly IPushNotificationRegistrationService _pnrService = pnrService;
    public async Task Handle(DeleteRegistrationsByIdentityAddressCommand request, CancellationToken cancellationToken)
    {
        await _pnrService.DeleteRegistrationsByIdentityAddress(request.IdentityAddress, cancellationToken);
    }
}
