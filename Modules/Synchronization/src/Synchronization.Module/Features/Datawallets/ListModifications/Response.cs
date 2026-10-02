using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.ListModifications;

public class Response : PagedResponse<DatawalletModificationDTO>
{
    public Response(IEnumerable<DatawalletModificationDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords)
    {
    }
}
