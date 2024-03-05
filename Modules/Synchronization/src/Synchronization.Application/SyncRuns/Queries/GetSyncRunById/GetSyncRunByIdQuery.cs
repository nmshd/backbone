using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class GetSyncRunByIdQuery : IRequest<SyncRunDTO>
{
    [JsonConstructor]
    public GetSyncRunByIdQuery(SyncRunId syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public SyncRunId SyncRunId { get; set; }
}
