using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.ListExternalEventsOfSyncRun;

public class ListExternalEventsOfSyncRunResponse : PagedResponse<ExternalEventDTO>
{
    public ListExternalEventsOfSyncRunResponse(IEnumerable<ExternalEventDTO> events, PaginationFilter previousPaginationFilter, int totalRecords) : base(events, previousPaginationFilter, totalRecords)
    {
    }
}
