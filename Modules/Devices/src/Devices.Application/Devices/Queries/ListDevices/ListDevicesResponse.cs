using Backbone.Modules.Devices.Application.Devices.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class ListDevicesResponse : PagedResponse<DeviceDTO>
{
    public ListDevicesResponse(IEnumerable<DeviceDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
