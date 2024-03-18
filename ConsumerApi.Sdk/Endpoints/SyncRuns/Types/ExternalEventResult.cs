namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class ExternalEventResult
{
    public required string ExternalEventId { get; set; }
    public string? ErrorCode { get; set; }
}
