using System.Text.Json.Serialization;
using MediatR;
using Synchronization.Application.SyncRuns.DTOs;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class GetSyncRunByIdQuery : IRequest<SyncRunDTO>
{
    [JsonConstructor]
    public GetSyncRunByIdQuery(SyncRunId syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public SyncRunId SyncRunId { get; set; }
}
