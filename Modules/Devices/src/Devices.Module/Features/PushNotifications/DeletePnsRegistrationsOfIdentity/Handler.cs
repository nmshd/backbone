using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;

public class Handler : IRequestHandler<Command>
{
    private readonly IPnsRegistrationsRepository _pnsRegistrationRepository;

    public Handler(IPnsRegistrationsRepository pnsRegistrationRepository)
    {
        _pnsRegistrationRepository = pnsRegistrationRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _pnsRegistrationRepository.Delete(PnsRegistration.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
