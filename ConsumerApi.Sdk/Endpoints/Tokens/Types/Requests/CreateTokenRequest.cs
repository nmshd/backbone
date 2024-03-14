namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;

public class CreateTokenRequest
{
    public required byte[] Content { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
