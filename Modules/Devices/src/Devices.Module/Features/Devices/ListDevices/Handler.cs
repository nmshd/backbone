using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Abstractions;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.ListDevices;

public class Handler : IRequestHandler<ListDevicesQuery, ListDevicesResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IUserContext userContext, IIdentitiesRepository devicesRepository)
    {
        _activeIdentity = userContext.GetAddress();
        _identitiesRepository = devicesRepository;
    }

    public async Task<ListDevicesResponse> Handle(ListDevicesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _identitiesRepository.ListDevicesOfIdentity(_activeIdentity, request.Ids.Select(DeviceId.Parse), request.PaginationFilter, cancellationToken);
        return new ListDevicesResponse(dbPaginationResult, request.PaginationFilter);
    }
}
