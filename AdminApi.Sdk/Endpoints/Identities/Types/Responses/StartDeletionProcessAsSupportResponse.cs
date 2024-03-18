namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;

public class StartDeletionProcessAsSupportResponse
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}
