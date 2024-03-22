using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;
public class Handler : IRequestHandler<DeletePnsRegistrationsOfIdentityCommand>
{
    private readonly IPnsRegistrationsRepository _pnsRegistrationRepository;

    public Handler(IPnsRegistrationsRepository pnsRegistrationRepository)
    {
        _pnsRegistrationRepository = pnsRegistrationRepository;
    }

    public async Task Handle(DeletePnsRegistrationsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _pnsRegistrationRepository.Delete(PnsRegistration.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
