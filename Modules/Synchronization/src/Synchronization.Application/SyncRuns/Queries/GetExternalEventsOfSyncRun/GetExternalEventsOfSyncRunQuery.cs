using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;

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
