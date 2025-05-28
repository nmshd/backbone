using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.ListModifications;

public class ListModificationsResponse : PagedResponse<DatawalletModificationDTO>
{
    public ListModificationsResponse(IEnumerable<DatawalletModificationDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords)
    {
    }
}
