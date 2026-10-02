using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.ListExternalEventsOfSyncRun;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListExternalEventsOfSyncRunQuery")]
public class Query : IRequest<Response>
{
    public required string SyncRunId { get; init; }
    public required PaginationFilter PaginationFilter { get; init; }
}
