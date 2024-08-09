using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class ListDevicesQuery : IRequest<ListDevicesResponse>
{
    public ListDevicesQuery(PaginationFilter paginationFilter, IEnumerable<string> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<string> Ids { get; set; }
}
