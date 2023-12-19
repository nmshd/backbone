using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevicesByIdentityAddress;

public class Handler(IIdentitiesRepository identitiesRepository) : IRequestHandler<DeleteDevicesByIdentityAddressCommand>
{
    public async Task Handle(DeleteDevicesByIdentityAddressCommand request, CancellationToken cancellationToken)
    {
        await identitiesRepository.DeleteDevices(Device.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
