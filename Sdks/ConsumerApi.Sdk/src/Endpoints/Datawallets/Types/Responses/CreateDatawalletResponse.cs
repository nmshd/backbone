namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;
public class CreateDatawalletResponse
{
    public required string DatawalletId { get; set; }
    public required string Owner { get; set; }
    public required ushort Version { get; set; }
}
