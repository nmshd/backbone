using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;

public class GetModificationsResponse : PagedResponse<DatawalletModificationDTO>
{
    public GetModificationsResponse(IEnumerable<DatawalletModificationDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
