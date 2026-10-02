using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.ListExternalEventsOfSyncRun;

public class ListExternalEventsOfSyncRunResponse : PagedResponse<ExternalEventDTO>
{
    public ListExternalEventsOfSyncRunResponse(IEnumerable<ExternalEventDTO> events, PaginationFilter previousPaginationFilter, int totalRecords) : base(events, previousPaginationFilter, totalRecords)
    {
    }
}
