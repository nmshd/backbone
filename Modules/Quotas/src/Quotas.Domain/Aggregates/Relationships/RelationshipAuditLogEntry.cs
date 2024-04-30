namespace Backbone.Modules.Quotas.Domain.Aggregates.Relationships;

public class RelationshipAuditLogEntry
{
    // ReSharper disable once UnusedMember.Local
    private RelationshipAuditLogEntry()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
    }

    public string Id { get; set; }
    public RelationshipAuditLogEntryReason Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum RelationshipAuditLogEntryReason
{
    Creation = 0,
    AcceptanceOfCreation = 1,
    RejectionOfCreation = 2,
    RevocationOfCreation = 3,
    Termination = 4,
    Reactivation = 5,
    AcceptanceOfReactivation = 6,
    RejectionOfReactivation = 7,
    RevocationOfReactivation = 8
}
