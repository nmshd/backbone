namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

public class UpdateTokenContentResponse
{
    public required string Id { get; init; }

    public required string? CreatedBy { get; init; }
    public required string? CreatedByDevice { get; init; }

    public required string? ForIdentity { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }

    public required bool IsPasswordProtected { get; init; }
}
