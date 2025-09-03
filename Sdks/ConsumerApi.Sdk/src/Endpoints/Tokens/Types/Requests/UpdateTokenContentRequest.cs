namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;

public class UpdateTokenContentRequest
{
    public required byte[] NewContent { get; init; }
    public byte[]? Password { get; init; }
}
