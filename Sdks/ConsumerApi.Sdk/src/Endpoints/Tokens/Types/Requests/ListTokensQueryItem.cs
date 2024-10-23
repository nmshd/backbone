namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;

public class ListTokensQueryItem
{
    public required string Id { get; set; }
    public byte[]? Password { get; set; }
}
