using AutoMapper;
using Backbone.Modules.Devices.Application.Devices.DTOs;

using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class Handler : IRequestHandler<ListDevicesQuery, ListDevicesResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IDevicesDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IDevicesRepository _devicesRepository;

    public Handler(IDevicesDbContext dbContext, IMapper mapper, IUserContext userContext, IDevicesRepository devicesRepository)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
        _devicesRepository = devicesRepository;
    }

    public async Task<ListDevicesResponse> Handle(ListDevicesQuery request, CancellationToken cancellationToken)
    {

        var dbPaginationResult = await _devicesRepository.FindAll(_activeIdentity, request.Ids, request.PaginationFilter);

        var items = _mapper.Map<DeviceDTO[]>(dbPaginationResult.ItemsOnPage);

        return new ListDevicesResponse(items, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
