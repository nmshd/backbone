using AutoMapper;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Queries.GetActiveDevice;

public class Handler : IRequestHandler<GetActiveDeviceQuery, DeviceDTO>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IMapper mapper, IUserContext userContext, IIdentitiesRepository identitiesRepository)
    {
        _mapper = mapper;
        _userContext = userContext;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<DeviceDTO> Handle(GetActiveDeviceQuery request, CancellationToken cancellationToken)
    {
        var device = await _identitiesRepository.GetDeviceById(_userContext.GetDeviceId(), cancellationToken);
        var deviceDTO = _mapper.Map<DeviceDTO>(device);
        return deviceDTO;
    }
}
