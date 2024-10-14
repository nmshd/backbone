namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;

public class TokenQueryItem
{
    public required string Id { get; set; }
    public byte[]? Password { get; set; }
}
