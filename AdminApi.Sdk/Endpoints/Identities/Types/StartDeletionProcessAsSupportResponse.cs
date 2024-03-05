namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

public class StartDeletionProcessAsSupportResponse
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
