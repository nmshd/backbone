using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Devices.DTOs;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class ListDevicesResponse : PagedResponse<DeviceDTO>
{
    public ListDevicesResponse(IEnumerable<DeviceDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
