using System.Text.Json.Serialization;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;

public class GetExternalEventsOfSyncRunQuery : IRequest<GetExternalEventsOfSyncRunResponse>
{
    [JsonConstructor]
    public GetExternalEventsOfSyncRunQuery(SyncRunId syncRunId, PaginationFilter paginationFilter)
    {
        PaginationFilter = paginationFilter;
        SyncRunId = syncRunId;
    }

    public SyncRunId SyncRunId { get; set; }
    public PaginationFilter PaginationFilter { get; set; }
}
