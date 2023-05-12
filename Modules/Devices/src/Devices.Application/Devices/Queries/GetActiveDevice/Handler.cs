using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Application.Devices.Queries.GetActiveDevice;

public class Handler : IRequestHandler<GetActiveDeviceQuery, DeviceDTO>
{
    private readonly IDevicesDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IDevicesRepository _devicesRepository;

    public Handler(IDevicesDbContext dbContext, IMapper mapper, IUserContext userContext, IDevicesRepository devicesRepository)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContext = userContext;
        _devicesRepository = devicesRepository;
    }

    public async Task<DeviceDTO> Handle(GetActiveDeviceQuery request, CancellationToken cancellationToken)
    {
        var device = await _devicesRepository.GetCurrentDevice(_userContext.GetDeviceId(), cancellationToken);
        var deviceDTO = _mapper.Map<DeviceDTO>(device);
        return deviceDTO;
    }
}
