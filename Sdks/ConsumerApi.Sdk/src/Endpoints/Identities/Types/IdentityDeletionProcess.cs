namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types;

public class IdentityDeletionProcess
{
    public required string Id { get; set; }
    public required List<IdentityDeletionProcessAuditLogEntry> AuditLog { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string CreatedByDevice { get; set; }

    public DateTime? GracePeriodEndsAt { get; set; }

    public DateTime? GracePeriodReminder1SentAt { get; set; }
    public DateTime? GracePeriodReminder2SentAt { get; set; }
    public DateTime? GracePeriodReminder3SentAt { get; set; }
}

public class IdentityDeletionProcessAuditLogEntry
{
    public required string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Message { get; set; }
    public string? OldStatus { get; set; }
    public required string NewStatus { get; set; }
}
