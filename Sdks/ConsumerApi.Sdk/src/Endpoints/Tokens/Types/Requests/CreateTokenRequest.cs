namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;

public class CreateTokenRequest
{
    public byte[]? Content { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public string? ForIdentity { get; set; }
    public byte[]? Password { get; set; }
}
