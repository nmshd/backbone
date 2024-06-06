namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;

public class CancelDeletionAsSupportResponse
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public required DateTime CancelledAt { get; set; }
}
