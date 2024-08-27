using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class GetSyncRunByIdQuery : IRequest<SyncRunDTO>
{
    [JsonConstructor]
    public GetSyncRunByIdQuery(string syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public string SyncRunId { get; set; }
}
