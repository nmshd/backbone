using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteRegistrationsByIdentityAddress;
public class Handler(IPnsRegistrationsRepository pnsRegistrationRepository) : IRequestHandler<DeleteRegistrationsByIdentityAddressCommand>
{
    public async Task Handle(DeleteRegistrationsByIdentityAddressCommand request, CancellationToken cancellationToken)
    {
        await pnsRegistrationRepository.DeleteByIdentityAddress(request.IdentityAddress, cancellationToken);
    }
}
