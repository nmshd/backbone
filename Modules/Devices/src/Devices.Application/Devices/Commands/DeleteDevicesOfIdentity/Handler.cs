using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevicesOfIdentity;

public class Handler(IIdentitiesRepository identitiesRepository) : IRequestHandler<DeleteDevicesOfIdentityCommand>
{
    public async Task Handle(DeleteDevicesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await identitiesRepository.DeleteDevices(Device.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
