using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class ListDevicesQuery : IRequest<ListDevicesResponse>
{
    public ListDevicesQuery(PaginationFilter paginationFilter, IEnumerable<DeviceId> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<DeviceId> Ids { get; set; }
}
