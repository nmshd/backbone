using System.Text.Json.Serialization;
using Backbone.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class GetSyncRunByIdQuery : IRequest<SyncRunDTO>
{
    [JsonConstructor]
    public GetSyncRunByIdQuery(SyncRunId syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public SyncRunId SyncRunId { get; set; }
}
