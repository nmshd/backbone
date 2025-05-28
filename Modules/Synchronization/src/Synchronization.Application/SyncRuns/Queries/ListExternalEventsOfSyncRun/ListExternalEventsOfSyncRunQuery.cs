using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.ListExternalEventsOfSyncRun;

public class ListExternalEventsOfSyncRunQuery : IRequest<ListExternalEventsOfSyncRunResponse>
{
    [JsonConstructor]
    public ListExternalEventsOfSyncRunQuery(string syncRunId, PaginationFilter paginationFilter)
    {
        PaginationFilter = paginationFilter;
        SyncRunId = syncRunId;
    }

    public string SyncRunId { get; set; }
    public PaginationFilter PaginationFilter { get; set; }
}
