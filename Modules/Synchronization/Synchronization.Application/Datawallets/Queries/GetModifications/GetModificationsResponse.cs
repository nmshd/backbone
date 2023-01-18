using Enmeshed.BuildingBlocks.Application.Pagination;
using Synchronization.Application.Datawallets.DTOs;

namespace Synchronization.Application.Datawallets.Queries.GetModifications;

public class GetModificationsResponse : PagedResponse<DatawalletModificationDTO>
{
    public GetModificationsResponse(IEnumerable<DatawalletModificationDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
