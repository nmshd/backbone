using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("FinalizeExternalEventSyncSyncRunCommand")]
public class Command : IRequest<Response>
{
    public required string SyncRunId { get; init; }
    public List<ExternalEventResult> ExternalEventResults { get; set; } = [];
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];

    public class ExternalEventResult
    {
        [JsonConstructor]
        public ExternalEventResult(string externalEventId, string? errorCode = null)
        {
            ExternalEventId = externalEventId;
            ErrorCode = errorCode;
        }

        public string ExternalEventId { get; set; }
        public string? ErrorCode { get; set; }
    }
}
