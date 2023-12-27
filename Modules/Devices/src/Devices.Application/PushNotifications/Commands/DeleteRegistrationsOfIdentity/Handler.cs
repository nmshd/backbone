using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteRegistrationsOfIdentity;
public class Handler(IPnsRegistrationsRepository pnsRegistrationRepository) : IRequestHandler<DeleteRegistrationsOfIdentityCommand>
{
    public async Task Handle(DeleteRegistrationsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await pnsRegistrationRepository.DeleteIPnsRegistrations(PnsRegistration.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
