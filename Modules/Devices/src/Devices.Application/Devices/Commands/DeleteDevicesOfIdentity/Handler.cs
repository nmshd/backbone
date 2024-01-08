using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevicesOfIdentity;

public class Handler : IRequestHandler<DeleteDevicesOfIdentityCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(DeleteDevicesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _identitiesRepository.DeleteDevices(Device.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
