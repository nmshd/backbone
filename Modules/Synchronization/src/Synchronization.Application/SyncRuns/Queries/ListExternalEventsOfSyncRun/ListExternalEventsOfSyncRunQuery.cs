using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.ListExternalEventsOfSyncRun;

public class ListExternalEventsOfSyncRunQuery : IRequest<ListExternalEventsOfSyncRunResponse>
{
    public required string SyncRunId { get; init; }
    public required PaginationFilter PaginationFilter { get; init; }
}
