using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Queries.GetActiveDevice;

public class Handler : IRequestHandler<GetActiveDeviceQuery, DeviceDTO>
{
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IUserContext userContext, IIdentitiesRepository identitiesRepository)
    {
        _userContext = userContext;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<DeviceDTO> Handle(GetActiveDeviceQuery request, CancellationToken cancellationToken)
    {
        var device = await _identitiesRepository.Get(_userContext.GetDeviceId(), cancellationToken) ?? throw new NotFoundException(nameof(Device));
        return new DeviceDTO(device);
    }
}
