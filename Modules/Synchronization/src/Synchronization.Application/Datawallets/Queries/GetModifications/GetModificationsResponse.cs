using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;

public class GetModificationsResponse : PagedResponse<DatawalletModificationDTO>
{
    public GetModificationsResponse(IEnumerable<DatawalletModificationDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
