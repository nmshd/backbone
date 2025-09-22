using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class GetSyncRunByIdQuery : IRequest<SyncRunDTO>
{
    public required string SyncRunId { get; init; }
}
