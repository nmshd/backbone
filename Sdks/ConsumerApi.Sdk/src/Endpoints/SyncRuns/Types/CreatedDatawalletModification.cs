namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class CreatedDatawalletModification
{
    public required string Id { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
}
