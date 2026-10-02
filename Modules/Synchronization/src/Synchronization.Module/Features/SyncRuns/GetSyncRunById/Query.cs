using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.GetSyncRunById;

public class Query : IRequest<SyncRunDTO>
{
    public required string SyncRunId { get; init; }
}
