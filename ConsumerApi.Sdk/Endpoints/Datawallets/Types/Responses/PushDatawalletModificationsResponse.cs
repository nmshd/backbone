namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;

public class PushDatawalletModificationsResponse
{
    public required long NewIndex { get; set; }
    public required List<PushDatawalletModificationsResponseItem> Modifications { get; set; }
}

public class PushDatawalletModificationsResponseItem
{
    public required string Id { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
}
