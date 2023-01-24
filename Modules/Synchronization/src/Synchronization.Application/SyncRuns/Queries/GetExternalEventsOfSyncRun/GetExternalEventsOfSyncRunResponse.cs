using Enmeshed.BuildingBlocks.Application.Pagination;
using Synchronization.Application.SyncRuns.DTOs;

namespace Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;

public class GetExternalEventsOfSyncRunResponse : PagedResponse<ExternalEventDTO>
{
    public GetExternalEventsOfSyncRunResponse(IEnumerable<ExternalEventDTO> events, PaginationFilter previousPaginationFilter, int totalRecords) : base(events, previousPaginationFilter, totalRecords) { }
}
