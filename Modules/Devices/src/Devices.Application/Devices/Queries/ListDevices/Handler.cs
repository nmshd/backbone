using AutoMapper;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
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

    public Handler(IDevicesDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<ListDevicesResponse> Handle(ListDevicesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .SetReadOnly<Device>()
            .NotDeleted()
            .OfIdentity(_activeIdentity);

        if (request.Ids.Any())
            query = query.WithIdIn(request.Ids);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);

        var items = _mapper.Map<DeviceDTO[]>(dbPaginationResult.ItemsOnPage);

        return new ListDevicesResponse(items, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
