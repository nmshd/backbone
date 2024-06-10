namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

public class IdentityDeletionProcessAuditLogEntryDTO
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string MessageKey { get; set; }
    public required string? OldStatus { get; set; }
    public required string NewStatus { get; set; }
}
