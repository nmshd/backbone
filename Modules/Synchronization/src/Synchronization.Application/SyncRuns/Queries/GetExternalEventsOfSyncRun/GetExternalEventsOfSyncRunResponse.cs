using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;

public class GetExternalEventsOfSyncRunResponse : PagedResponse<ExternalEventDTO>
{
    public GetExternalEventsOfSyncRunResponse(IEnumerable<ExternalEventDTO> events, PaginationFilter previousPaginationFilter, int totalRecords) : base(events, previousPaginationFilter, totalRecords) { }
}
