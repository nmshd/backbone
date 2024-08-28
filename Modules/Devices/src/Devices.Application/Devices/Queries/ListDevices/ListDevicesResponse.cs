using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class ListDevicesResponse : PagedResponse<DeviceDTO>
{
    public ListDevicesResponse(DbPaginationResult<Device> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(dbPaginationResult.ItemsOnPage.Select(d => new DeviceDTO(d)),
        previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
