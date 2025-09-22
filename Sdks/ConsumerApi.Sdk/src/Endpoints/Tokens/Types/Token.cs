namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types;

public class Token
{
    public required string Id { get; set; }
    public required string? CreatedBy { get; set; }
    public required string? CreatedByDevice { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public string? ForIdentity { get; set; }
    public required byte[]? Content { get; set; }
}
