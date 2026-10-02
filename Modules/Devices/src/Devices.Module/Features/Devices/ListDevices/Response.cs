using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;

namespace Backbone.Modules.Devices.Module.Features.Devices.ListDevices;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListDevicesResponse")]
public class Response : PagedResponse<DeviceDTO>
{
    public Response(DbPaginationResult<Device> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(dbPaginationResult.ItemsOnPage.Select(d => new DeviceDTO(d)),
        previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
