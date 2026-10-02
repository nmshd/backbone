using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.GetActiveDevice;

public class Handler : IRequestHandler<Query, DeviceDTO>
{
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IUserContext userContext, IIdentitiesRepository identitiesRepository)
    {
        _userContext = userContext;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<DeviceDTO> Handle(Query request, CancellationToken cancellationToken)
    {
        var device = await _identitiesRepository.Get(_userContext.GetDeviceId(), cancellationToken) ?? throw new NotFoundException(nameof(Device));
        return new DeviceDTO(device);
    }
}
