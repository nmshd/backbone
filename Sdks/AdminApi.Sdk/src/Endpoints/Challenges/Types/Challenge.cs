namespace Backbone.AdminApi.Sdk.Endpoints.Challenges.Types;

public class Challenge
{
    public required string Id { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
}
