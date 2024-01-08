using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteRegistrationsOfIdentity;
public class Handler : IRequestHandler<DeleteRegistrationsOfIdentityCommand>
{
    private readonly IPnsRegistrationsRepository _pnsRegistrationRepository;

    public Handler(IPnsRegistrationsRepository pnsRegistrationRepository)
    {
        _pnsRegistrationRepository = pnsRegistrationRepository;
    }

    public async Task Handle(DeleteRegistrationsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _pnsRegistrationRepository.DeleteIPnsRegistrations(PnsRegistration.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
